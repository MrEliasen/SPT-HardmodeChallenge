using System.Collections;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Services;
using Vagabond.Common;
using Vagabond.Common.Data;
using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;
using Location = SPTarkov.Server.Core.Models.Eft.Common.Location;

namespace Vagabond.Server.Services;

internal static class ExfilService
{
    public static Dictionary<RaidLocation, Dictionary<string, List<CustomExfilDefinition>>> CustomExfils = new();

    public static void Apply(DatabaseService databaseService)
    {
        foreach (var loc in Enum.GetValues(typeof(RaidLocation)).Cast<RaidLocation>())
        {
            if (loc ==  RaidLocation.Nil)
            {
                continue;
            }

            LocationData.InverseLookupTable.TryGetValue(loc, out var maps);
            var ent = new Dictionary<string, List<CustomExfilDefinition>>();
            foreach (var map in maps)
            {
                ent.Add(map, new List<CustomExfilDefinition>());
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

    private static void AddOrReplaceExtract(Location location, CustomExfilDefinition definition)
    {
        var allExtracts = location.AllExtracts?.ToList() ?? new List<AllExtractsExit>();
        allExtracts.RemoveAll(x => string.Equals(x.Name, definition.DisplayName, StringComparison.OrdinalIgnoreCase)
                                   || string.Equals(x.SptName, definition.Identifier, StringComparison.OrdinalIgnoreCase));
        allExtracts.Add(CreateExit(definition));
        location.AllExtracts = allExtracts;

        var baseExits = location.Base.Exits?.ToList() ?? new List<Exit>();
        baseExits.RemoveAll(x => string.Equals(x.Name, definition.DisplayName, StringComparison.OrdinalIgnoreCase));
        baseExits.Add(CreateExit(definition));
        location.Base.Exits = baseExits;
    }

    private static void AddOrReplaceTransit(Location location, CustomExfilDefinition definition)
    {
        var transits = location.Base.Transits?.ToList() ?? new List<Transit>();
        transits.RemoveAll(x => string.Equals(x.Name, definition.DisplayName, StringComparison.OrdinalIgnoreCase)
                                || (definition.TransitPointId.HasValue && x.Id == definition.TransitPointId.Value));
        transits.Add(new Transit
        {
            Name = definition.DisplayName,
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

    private static AllExtractsExit CreateExit(CustomExfilDefinition definition)
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
        var entryPoints = location.Base.Exits?
            .Where(x => string.Equals(x.Side, "Pmc", StringComparison.OrdinalIgnoreCase)
                        && !string.IsNullOrWhiteSpace(x.EntryPoints))
            .SelectMany(x => x.EntryPoints.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray() ?? Array.Empty<string>();

        return string.Join(",", entryPoints);
    }
}
