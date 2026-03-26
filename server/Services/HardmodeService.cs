using HardmodeChallenge.Server.Config;
using HardmodeChallenge.Server.Definitions;
using HardmodeChallenge.Server.Models.Enums;
using HardmodeChallenge.Server.State;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Inventory;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Servers;

namespace HardmodeChallenge.Server.Services;

internal static class HardmodeService
{
    public const string Roubles = "5449016a4bdc2d6f028b456f";
    public const string SpectatorTraderID = "686172646d6f647472616465";

    public static bool IsMapCompleted(List<string> completedRaids, string locationName)
    {
        LocationData.LookupTable.TryGetValue(locationName, out var mapRef);
        return completedRaids.Any(x => string.Equals(x, mapRef.ToString(), StringComparison.OrdinalIgnoreCase));
    }

    public static bool HasCompletedAllMaps(List<string> completedRaids)
    {
        foreach (var raid in LocationData.Locations)
        {
            if (!completedRaids.Any(x => string.Equals(x, raid.Key.ToString(), StringComparison.OrdinalIgnoreCase)))
            {
                if (!HardmodeConfig._config.IsLabsRequired && raid.Key == HCLocation.Labs)
                {
                    continue;
                }

                if (!HardmodeConfig._config.IsLabyrinthRequired && raid.Key == HCLocation.Labyrinth)
                {
                    continue;
                }

                return false;
            }
        }

        return true;
    }

    public static void ResetProfile(MongoId sessionId, PmcData pmc, bool keepSecureContainer = false, bool softReset = false)
    {
        var inventory = pmc.Inventory;
        if (inventory == null)
        {
            HardmodeLogger.Error("PlayerCompletedChallenge: inventory was null.");
            return;
        }

        var items = inventory.Items;
        if (items == null)
        {
            HardmodeLogger.Error("PlayerCompletedChallenge: inventory items list was null.");
            return;
        }
        
        ApplyTraderRestrictions(pmc, true);

        var state = HardmodeState.GetState(sessionId);
        state.ProfileInitialized = true;
        state.HasEnteredFirstRaid = false;
        state.RaidEntryCount = 0;
        state.ResetProfile = false;
        state.CompletedRaids = [];
        HardmodeState.SaveState(sessionId, state);

        if (softReset)
        {
            HardmodeLogger.Log($"ResetProfile: player profile soft reset {sessionId}.");
            return;
        }

        WipeItems(sessionId, pmc, state.ChallengesCompleted, true, true, true, keepSecureContainer);
        AddMoney(sessionId, pmc);
        HardmodeLogger.Log($"ResetProfile: player profile reset {sessionId}.");
    }

    public static SptProfile? GetPmcProfile(MongoId sessionId)
    {
        var server = ReflectionUtil.GetService<SaveServer>();
        if (server == null)
        {
            return null;
        }

        return server.GetProfile(sessionId);
    }

    public static void PersistProfileIfPossible(MongoId sessionId)
    {
        try
        {
            var server = ReflectionUtil.GetService<SaveServer>();
            if (server == null)
            {
                return;
            }

            server.SaveProfileAsync(sessionId);
        }
        catch (Exception ex)
        {
            HardmodeLogger.Error($"PersistProfileIfPossible failed: {ex}");
        }
    }

    public static void WipeItems(MongoId sessionId, PmcData pmc,int hcLevel, bool wipeEquipment = false,
        bool wipeStash = false, bool forceRemoveAllMoney = false, bool keepSecureContainer = false)
    {
        var inventory = pmc.Inventory;
        if (inventory == null)
        {
            HardmodeLogger.Error("Wipe Equipment: inventory was null.");
            return;
        }

        if (inventory.Items == null)
        {
            HardmodeLogger.Error("Wipe Equipment: inventory items list was null.");
            return;
        }

        if (inventory.Stash == null)
        {
            HardmodeLogger.Error("Wipe Equipment: Stash ID null");
            return;
        }

        var invHelper = ReflectionUtil.GetService<InventoryHelper>();
        if (invHelper == null)
        {
            HardmodeLogger.Error("Wipe Equipment: Inventory Helper not found");
            return;
        }

        var idsToRemove = new List<MongoId>();
        var currencies = new List<string>([
            "5696686a4bdc2da3298b456a", // Dollars
            "569668774bdc2da2298b4568", // Euros
            "5449016a4bdc2d6f028b456f" // Rubs
        ]);

        HashSet<String> securedContainerIdList = new(StringComparer.OrdinalIgnoreCase);
        if (keepSecureContainer)
        {
            securedContainerIdList = SecureContainerService.GetSecureContainerItemIdsToKeep(inventory.Items);
        }

        foreach (var item in inventory.Items)
        {
            if (item.Id == inventory.Stash)
            {
                continue;
            }

            // Keep the pockets container itself
            if (string.Equals(item.SlotId, "Pockets", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // Keep the secure container
            if (keepSecureContainer && securedContainerIdList.Contains(item.Id))
            {
                continue;
            }

            if (forceRemoveAllMoney &&
                currencies.Any(x => string.Equals(x, item.Template, StringComparison.OrdinalIgnoreCase)))
            {
                idsToRemove.Add(item.Id);
                continue;
            }

            if (wipeStash && item.ParentId != null && item.ParentId == inventory.Stash)
            {
                idsToRemove.Add(item.Id);
                continue;
            }

            if (wipeEquipment && item.ParentId != null && item.ParentId != inventory.Stash)
            {
                idsToRemove.Add(item.Id);
            }
        }
        
        foreach (var id in idsToRemove.Distinct())
        {
            invHelper.RemoveItem(pmc, id, sessionId);
        }
    }

    public static void ApplyTraderRestrictions(PmcData pmc, bool isNewAccount = false)
    {
        var tradersInfo = pmc?.TradersInfo;
        if (tradersInfo == null)
        {
            return;
        }

        foreach (KeyValuePair<MongoId, TraderInfo> entry in tradersInfo)
        {
            entry.Value.Disabled = true;
            entry.Value.Unlocked = false;
            
            if (HardmodeConfig._config.AddSpectatorTrader && entry.Key == SpectatorTraderID)
            {
                entry.Value.Disabled = false;
                entry.Value.Unlocked = true;
            }
        
            if (HardmodeConfig._config.PermanentTraders.Contains(entry.Key))
            {
                entry.Value.Disabled = false;
                entry.Value.Unlocked = true;
            }

            if (isNewAccount && HardmodeConfig._config.StarterTraders.Contains(entry.Key))
            {
                entry.Value.Disabled = false;
                entry.Value.Unlocked = true;
            }
        }
    }

    public static bool ShouldApplyHardmodeRules(MongoId sessionId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return false;
            }

            if (HardmodeConfig._config.IgnoredProfiles.Contains(sessionId))
            {
                return false;
            }

            var pmc = GetPmcProfile(sessionId);
            if (pmc == null)
            {
                return false;
            }

            if (pmc.ProfileInfo == null)
            {
                return false;
            }

            var isHeadless = pmc.ProfileInfo.Username?.StartsWith("headless_") ?? false;
            return !isHeadless;
        }
        catch
        {
            return false;
        }
    }


    public static void AddMoney(MongoId sessionId, PmcData pmcData)
    {
        var invHelper = ReflectionUtil.GetService<InventoryHelper>();
        var eventOutputHolder = ReflectionUtil.GetService<EventOutputHolder>();

        if (invHelper == null || eventOutputHolder == null)
        {
            HardmodeLogger.Error("AddMoney: required service was null.");
            return;
        }

        var amount = HardmodeConfig._config.StartingRoubles;
        while (amount > 0)
        {
            var moneyItem = new Item
            {
                Id = new MongoId(),
                Template = Roubles,
                Upd = new Upd
                {
                    StackObjectsCount = amount > 500_000 ? 500_000 : amount,
                }
            };

            amount -= (int)(moneyItem.Upd?.StackObjectsCount ?? 500_000);

            var request = new AddItemDirectRequest
            {
                ItemWithModsToAdd = new List<Item> { moneyItem }
            };

            invHelper.AddItemToStash(sessionId, request, pmcData, eventOutputHolder.GetOutput(sessionId));
        }
    }
}