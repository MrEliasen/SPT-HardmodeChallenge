using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using Vagabond.Common.Data;
using Vagabond.Common.Enums;
using Vagabond.Server.Definitions;
using Vagabond.Server.State;

namespace Vagabond.Server.Services;

internal static class HideoutService
{
    private static readonly List<TraderLocation> TraderLocations = new()
    {
        new TraderLocation
        {
            Id = "54cb57776803fa99248b456e", // Therapist
            Raid = RaidLocation.GroundZero,
            ExitName = "VGB_EXT_THERAPIST",
        },
        new TraderLocation
        {
            Id = "58330581ace78e27b8b10cee", // Skier
            Raid = RaidLocation.Customs,
            ExitName = "VGB_EXT_SKIER",
        },
        new TraderLocation
        {
            Id = "5c0647fdd443bc2504c2d371", // Jaeger
            Raid = RaidLocation.Woods,
            ExitName = "VGB_EXT_JAEGER",
        },
        new TraderLocation
        {
            Id = "5a7c2eca46aef81a7ca2145d", // Mechanic
            Raid = RaidLocation.FactoryDay,
            ExitName = "VGB_EXT_MECHANIC",
        },
        new TraderLocation
        {
            Id = "5a7c2eca46aef81a7ca2145d", // Mechanic
            Raid = RaidLocation.FactoryNight,
            ExitName = "VGB_EXT_MECHANIC",
        },
        new TraderLocation
        {
            Id = "54cb50c76803fa8b248b4571", // Prapor
            Raid = RaidLocation.Streets,
            ExitName = "VGB_EXT_PRAPOR",
        },
        new TraderLocation
        {
            Id = "5ac3b934156ae10c4430e83c", // Ragman
            Raid = RaidLocation.Interchange,
            ExitName = "VGB_EXT_RAGMAN",
        },
        new TraderLocation
        {
            Id = "5935c25fb3acc3127c3d8cd9", // Peacekeeper
            Raid = RaidLocation.Shoreline,
            ExitName = "VGB_EXT_PEACEKEEPER",
        },
        new TraderLocation
        {
            Id = "579dc571d53a0658a154fbec", // Fence
            Raid = RaidLocation.Streets,
            ExitName = "VGB_EXT_FENCE",
        },
        new TraderLocation
        {
            Id = "579dc571d53a0658a154fbec", // Fence
            Raid = RaidLocation.Lighthouse,
            ExitName = "VGB_EXT_FENCE_DL",
        }
    };

    public static void UpdateTraderAccess(PmcData pmc, VagabondState state)
    {
        var traderId = "";
        var raidE = VagabondLocations.NormaliseMapName(state.CurrentMap);
        if (raidE != RaidLocation.Nil)
        {
            traderId = TraderLocations.FirstOrDefault(t => t.Raid == raidE && t.ExitName == state.LastExit)?.Id ?? "";
        }
        
        var tradersInfo = pmc.TradersInfo;
        foreach (KeyValuePair<MongoId, TraderInfo> entry in tradersInfo)
        {
            if (entry.Key == traderId)
            {
                entry.Value.Disabled = false;
                entry.Value.Unlocked = true;
                continue;
            }
            
            entry.Value.Disabled = true;
            entry.Value.Unlocked = false;
        }
    }
}