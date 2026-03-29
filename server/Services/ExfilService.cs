using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Services;
using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;
using Location = SPTarkov.Server.Core.Models.Eft.Common.Location;

namespace Vagabond.Server.Services;

internal static class ExfilService
{
    public static Dictionary<RaidLocation, HashSet<string>> CustomExfils = new ();
    
    public static void Apply(DatabaseService databaseService)
    {
        foreach (var loc in Enum.GetValues(typeof(RaidLocation)).Cast<RaidLocation>())
        {
            CustomExfils[loc] = new();
        }
        
        var locations = databaseService.GetLocations();
        AddShorelineExfils(locations.Shoreline);
        VagabondLogger.Log($"{CopyUtil.ToJson(CustomExfils)}");
    }

    private static void AddShorelineExfils(Location location)
    { 
        VagabondLogger.Log($"ADDING SHORELINE EXFILS");
        
        List<Transit> transits = location.Base.Transits?.ToList() ?? new();
        List<AllExtractsExit> allExtracts = location.AllExtracts?.ToList() ?? new();
       
       var newTransit = new Transit
        {
            Name = "Transit to Customs",
            Location = RaidLocation.Shoreline.ToString(),
            Target = LocationData.Locations[RaidLocation.Shoreline].First(),
            ActivateAfterSeconds = 60,
            Time = 20,
            IsActive = true,
            Events = false,
            HideIfNoKey = false,
        };
        VagabondLogger.Log($"ADDING NEW TRANSIT: {newTransit.Name}");
        CustomExfils[RaidLocation.Shoreline].Add(newTransit.Name);
        transits.Add(newTransit);

        var newAllExit = new AllExtractsExit
        {
            Chance = 100,
            ChancePVE = 100,
            Count = 0,
            CountPVE = 0,
            EntryPoints = "",
            EventAvailable = false,
            ExfiltrationTime = 20,
            ExfiltrationTimePVE = 20,
            ExfiltrationType = ExfiltrationType.Individual,
            Id = "",
            MaxTime = 0,
            MaxTimePVE = 0,
            MinTime = 0,
            MinTimePVE = 0,
            Name = "shoreline_TransitToCustoms",
            PassageRequirement = RequirementState.None,
            PlayersCount = 0,
            PlayersCountPVE = 0,
            RequiredSlot = EquipmentSlots.FirstPrimaryWeapon,
            RequirementTip = "",
            Side = "Pmc"
        };
        VagabondLogger.Log($"ADDING ALL NEW TRANSIT: {newAllExit.Name}");
        CustomExfils[RaidLocation.Shoreline].Add(newAllExit.Name);
        allExtracts.Add(newAllExit);
       
       location.Base.Transits = transits;
       location.AllExtracts = allExtracts;
    }
}