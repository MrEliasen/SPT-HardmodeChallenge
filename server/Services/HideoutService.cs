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
            Raid = RaidLocation.Streets,
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
            Id = "579dc571d53a0658a154fbec", // Jaeger
            Raid = RaidLocation.Woods,
            ExitName = "VGB_EXT_JAEGER",
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