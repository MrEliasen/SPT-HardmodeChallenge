using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Services;
using Vagabond.Common.Enums;
using Vagabond.Common.Models;
using Location = SPTarkov.Server.Core.Models.Eft.Common.Location;

namespace Vagabond.Server.Services;

internal static class ExfilService
{
    public static Dictionary<RaidLocation, List<CustomExfilDefinition>> CustomExfils = new();

    public static void Apply(DatabaseService databaseService)
    {
        foreach (var loc in Enum.GetValues(typeof(RaidLocation)).Cast<RaidLocation>())
        {
            CustomExfils[loc] = new();
        }

        var locations = databaseService.GetLocations();
        AddShorelineExfils(locations.Shoreline);
        VagabondLogger.Log(CopyUtil.ToJson(CustomExfils));
    }

    public static bool IsCustomExfilName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        return CustomExfils.Values
            .SelectMany(x => x)
            .Any(x => string.Equals(x.DisplayName, name, StringComparison.OrdinalIgnoreCase)
                      || string.Equals(x.Identifier, name, StringComparison.OrdinalIgnoreCase));
    }

    private static void AddShorelineExfils(Location location)
    {
        var entryPoints = GetPmcEntryPoints(location);
        var definition = new CustomExfilDefinition
        {
            Identifier = "vagabond_shoreline_cliff_vex",
            DisplayName = "Vagabond Cliff V-Ex",
            TemplateExitName = "Road to Customs",
            EntryPoints = entryPoints,
            ExfiltrationTime = 20f,
            X = -848.76f,
            Y = -42.364f,
            Z = 2.421f,
            RotationY = 30f,
            Side = "Pmc"
        };

        CustomExfils[RaidLocation.Shoreline].Add(definition);

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
