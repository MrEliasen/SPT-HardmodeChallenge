using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using Vagabond.Common.Data;
using Vagabond.Common.Enums;
using Vagabond.Server.Config;
using Vagabond.Server.Definitions;
using Vagabond.Server.State;

namespace Vagabond.Server.Services;

internal static class HideoutService
{
    public const string HideoutIdPrefix = "VGB_HO_";

    public const string HideoutNamePrefix = "Hideout Entrance";

    // access is not changed by extraction.
    private static readonly List<string> IgnoredTraders =
    [
        "656f0f98d80a697f855d34b1", // BTR Driver
        "688246518448b05efd61d461", // Mr. Kerman
        "638f541a29ffd1183d187f57", // Lightkeeper
        "68fe15990f29ba3fdbba9d55", // Radio station
        "68fe15910f29ba3fdbba9d54", // Taran
        "688246958448b05efd61d462", // Voevoda
    ];

    public static readonly List<TraderLocation> TraderLocations = new()
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

    public static IReadOnlyCollection<string> GetAllTraderIds()
    {
        return TraderLocations
            .Select(x => x.Id)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }

    public static string? GetCurrentTraderId(VagabondState state)
    {
        var raid = VagabondLocations.NormaliseMapName(state.CurrentMap);
        if (raid == RaidLocation.Nil || string.IsNullOrWhiteSpace(state.LastExit))
        {
            return null;
        }

        return TraderLocations.FirstOrDefault(x =>
            x.Raid == raid
            && string.Equals(x.ExitName, state.LastExit, StringComparison.OrdinalIgnoreCase))?.Id;
    }

    public static void UpdateTraderAccess(PmcData pmc, VagabondState state)
    {
        var traderId = GetCurrentTraderId(state) ?? string.Empty;
        var isCustomTraderLoc = state.LastExit == "VGB_EXT_MARKET";
        var traderIdList = TraderLocations.ToList().ConvertAll(x => x.Id);
        var tradersInfo = pmc.TradersInfo;
        var isOwnHideout = !string.IsNullOrEmpty(state.HideoutState?.Id) &&
                           state.LastExit == $"{HideoutIdPrefix}{state.HideoutState?.Id}";
        
        foreach (KeyValuePair<MongoId, TraderInfo> entry in tradersInfo)
        {
            if (IgnoredTraders.Contains(entry.Key))
            {
                continue;
            }

            if (VagabondConfig.Config.AddFenceToHideout && entry.Key == "579dc571d53a0658a154fbec")
            {
                if (state.LastExit.IndexOf(HideoutIdPrefix, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    entry.Value.Disabled = false;
                    entry.Value.Unlocked = true;
                    continue;
                }
            }
            
            if (isOwnHideout && state.HideoutTraders.Contains(entry.Key))
            {
                entry.Value.Disabled = false;
                entry.Value.Unlocked = true;
                continue;
            }

            if (entry.Key == traderId || (isCustomTraderLoc && !traderIdList.Contains(entry.Key)))
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