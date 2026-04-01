using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Services;
using Vagabond.Common.Data;
using Vagabond.Common.Definitions;
using Vagabond.Common.Interfaces;
using Vagabond.Common.Enums;
using Location = SPTarkov.Server.Core.Models.Eft.Common.Location;

namespace Vagabond.Server.Services;

internal static class ExfilService
{
    public static Dictionary<RaidLocation, Dictionary<string, List<CustomExfil>>> CustomExfils = new();

    public static void Apply(DatabaseService databaseService)
    {
        foreach (var loc in Enum.GetValues(typeof(RaidLocation)).Cast<RaidLocation>())
        {
            if (loc ==  RaidLocation.Nil)
            {
                continue;
            }
            
            if (!VagabondLocations.InverseLookupTable.TryGetValue(loc, out var maps))
            {
                continue;
            }
            
            var ent = new Dictionary<string, List<CustomExfil>>();
            foreach (var map in maps)
            {
                ent.Add(map, new List<CustomExfil>());
            }
            
            CustomExfils.Add(loc, ent);
        }
        
        var locations = databaseService.GetLocations();
        AddExtractions(9000, locations.Bigmap, new ExfilsCustoms()); 
        AddExtractions(9100, locations.Factory4Day, new ExfilsFactoryDay()); 
        AddExtractions(9200,locations.Factory4Night, new ExfilsFactoryNight()); 
        AddExtractions(9300,locations.SandboxHigh, new ExfilsGroundZero()); 
        AddExtractions(9400,locations.Interchange, new ExfilsInterchange()); 
        AddExtractions(9500,locations.Lighthouse, new ExfilsLighthouse()); 
        AddExtractions(9600,locations.RezervBase, new ExfilsReserve()); 
        AddExtractions(9700,locations.Shoreline, new ExfilsShoreline()); 
        AddExtractions(9800,locations.TarkovStreets, new ExfilsStreets()); 
        AddExtractions(9900,locations.Woods, new ExfilsWoods());
        AddExtractions(10000,locations.Laboratory, new ExfilsLabs()); 
        AddExtractions(1100,locations.Labyrinth, new ExfilsLabyrinth());
    }

    private static void AddExtractions(int pointIdOffset, Location location, ICustomExtilData data)
    {
        var pmcEntryPoints = GetPmcEntryPoints(location);

        foreach (var ext in data.Extracts)
        {
            ext.EntryPoints = pmcEntryPoints;
            CustomExfils[data.Raid][data.MapName].Add(ext);
            AddOrReplaceExtract(location, ext);
        }

        var i = 1;
        foreach (var transit in data.Transits)
        {
            transit.TransitPointId = pointIdOffset + i;
            CustomExfils[data.Raid][data.MapName].Add(transit);
            AddOrReplaceTransit(location, transit);
            i++;
        }
    }

    public static bool IsCustomExtractName(string? name, RaidLocation raid, string mapName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        return CustomExfils[raid][mapName]
            .Any(x => string.Equals(x.DisplayName, name, StringComparison.OrdinalIgnoreCase)
                      || string.Equals(x.Identifier, name, StringComparison.OrdinalIgnoreCase));
    }

    private static void AddOrReplaceExtract(Location location, CustomExfil definition)
    {
        var allExtracts = location.AllExtracts.ToList();
        allExtracts.RemoveAll(x => string.Equals(x.Name, definition.DisplayName, StringComparison.OrdinalIgnoreCase)
                                   || string.Equals(x.SptName, definition.Identifier, StringComparison.OrdinalIgnoreCase));
        allExtracts.Add(CreateExit(definition));
        location.AllExtracts = allExtracts;

        var baseExits = location.Base.Exits.ToList();
        baseExits.RemoveAll(x => string.Equals(x.Name, definition.DisplayName, StringComparison.OrdinalIgnoreCase));
        baseExits.Add(CreateExit(definition));
        location.Base.Exits = baseExits;
    }

    private static void AddOrReplaceTransit(Location location, CustomExfil definition)
    {
        var transits = location.Base.Transits?.ToList() ?? new List<Transit>();
        transits.RemoveAll(x =>
            string.Equals(x.Name, definition.Identifier, StringComparison.OrdinalIgnoreCase)
            || (definition.TransitPointId.HasValue && x.Id == definition.TransitPointId.Value));
        
        transits.Add(new Transit
        {
            Name = definition.Identifier,
            Description = definition.Description,
            Conditions = string.Empty,
            Id = definition.TransitPointId,
            Location = definition.DestinationLocation,
            Target = string.IsNullOrWhiteSpace(definition.TargetLocation) ? definition.DestinationLocation : definition.TargetLocation,
            ActivateAfterSeconds = definition.ActivateAfterSeconds,
            Time = (long)Math.Round(definition.ExfiltrationTime),
            IsActive = definition.IsActive,
            Events = definition.Events,
            HideIfNoKey = definition.HideIfNoKey
        });

        location.Base.Transits = transits;
    }

    private static AllExtractsExit CreateExit(CustomExfil definition)
    {
        return new AllExtractsExit
        {
            Name = definition.DisplayName,
            SptName = definition.Identifier,
            Chance = 100,
            ChancePVE = 100,
            Count = 0,
            CountPVE = 0,
            EntryPoints = definition.EntryPoints,
            EventAvailable = false,
            ExfiltrationTime = definition.ExfiltrationTime,
            ExfiltrationTimePVE = definition.ExfiltrationTime,
            ExfiltrationType = ExfiltrationType.Individual,
            Id = string.Empty,
            MaxTime = 0,
            MaxTimePVE = 0,
            MinTime = 0,
            MinTimePVE = 0,
            PassageRequirement = RequirementState.None,
            PlayersCount = 0,
            PlayersCountPVE = 0,
            RequiredSlot = EquipmentSlots.FirstPrimaryWeapon,
            RequirementTip = string.Empty,
            Side = definition.Side
        };
    }

    private static string GetPmcEntryPoints(Location location)
    {
        var entryPoints = location.Base.Exits
            .Where(x => string.Equals(x.Side, "Pmc", StringComparison.OrdinalIgnoreCase))
            .Select(x => x.EntryPoints)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .SelectMany(x => x!.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
            
        return string.Join(",", entryPoints);
    }

    public static ICustomExtilData GetCustomMapData(RaidLocation raid)
    {
        return (raid) switch
        {
            (RaidLocation.Customs) => new ExfilsCustoms(),
            (RaidLocation.FactoryDay) => new ExfilsFactoryDay(),
            (RaidLocation.FactoryNight) => new ExfilsFactoryNight(),
            (RaidLocation.GroundZero) => new ExfilsGroundZero(),
            (RaidLocation.Interchange) => new ExfilsInterchange(),
            (RaidLocation.Lighthouse) => new ExfilsLighthouse(),
            (RaidLocation.Reserve) => new ExfilsReserve(),
            (RaidLocation.Shoreline) => new ExfilsShoreline(),
            (RaidLocation.Streets) => new ExfilsStreets(),
            (RaidLocation.Woods) => new ExfilsWoods(),
            (RaidLocation.Labs) => new ExfilsLabs(),
            (RaidLocation.Labyrinth) => new ExfilsLabyrinth(),
            RaidLocation.Nil => throw new ArgumentOutOfRangeException(nameof(raid), raid, "RaidLocation.Nil has no custom map data."),
            _ => throw new ArgumentOutOfRangeException(nameof(raid), raid, "Unsupported raid location.")
        };
    }
}
