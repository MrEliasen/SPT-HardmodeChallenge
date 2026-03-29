using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Inventory;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using Vagabond.Server.Config;
using Vagabond.Server.Definitions;
using Vagabond.Server.Models.Enums;
using Vagabond.Server.State;
using Location = SPTarkov.Server.Core.Models.Eft.Common.Location;

namespace Vagabond.Server.Services;

internal static class ExfilService
{
    public static void Apply(DatabaseService databaseService)
    {
        var locations = databaseService.GetLocations();
        AddShorelineExfils(locations.Shoreline);
    }

    private static void AddShorelineExfils(Location location)
    { 
        List<Transit> transits = location.Base.Transits?.ToList() ?? new();
        List<AllExtractsExit> allExtracts = location.AllExtracts?.ToList() ?? new();
       
        transits.Add(new Transit
        {
            Name = "Transit to Customs",
            Location = RaidLocation.Customs.ToString(),
            Target = LocationData.Locations[RaidLocation.Customs].First(),
            ActivateAfterSeconds = 60,
            Time = 20,
            IsActive = true,
            Events = false,
            HideIfNoKey = false,
        });

        allExtracts.Add(new AllExtractsExit
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
            Name = "TransitToCustoms",
            PassageRequirement = RequirementState.None,
            PlayersCount = 0,
            PlayersCountPVE = 0,
            RequiredSlot = EquipmentSlots.FirstPrimaryWeapon,
            RequirementTip = "",
            Side = "Pmc"
        });
       
       location.Base.Transits = transits;
       location.AllExtracts = allExtracts;
    }
}