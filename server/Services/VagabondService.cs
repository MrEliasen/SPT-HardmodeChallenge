using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Inventory;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Servers;
using Vagabond.Common.Data;
using Vagabond.Server.Config;
using Vagabond.Common.Enums;
using Vagabond.Server.State;

namespace Vagabond.Server.Services;

internal static class VagabondService
{
    public const string Roubles = "5449016a4bdc2d6f028b456f";
    public const string SpectatorTraderID = "686172646d6f647472616465";

    public static void ResetProfile(MongoId sessionId, PmcData pmc, bool keepSecureContainer = false, bool softReset = false)
    {
        var inventory = pmc.Inventory;
        if (inventory == null)
        {
            VagabondLogger.Error("PlayerCompletedChallenge: inventory was null.");
            return;
        }

        var items = inventory.Items;
        if (items == null)
        {
            VagabondLogger.Error("PlayerCompletedChallenge: inventory items list was null.");
            return;
        }
        
        ApplyTraderRestrictions(pmc, true);

        var state = VagabondState.GetState(sessionId);
        state.ProfileInitialized = true;
        state.ResetProfile = false;
        state.CurrentMap = "";
        state.LastExit = "";
        state.TransitState = new TransitState();
        VagabondState.SaveState(sessionId, state);

        if (softReset)
        {
            VagabondLogger.Log($"ResetProfile: player profile soft reset {sessionId}.");
            return;
        }

        WipeItems(sessionId, pmc, true, true, true, keepSecureContainer);
        AddMoney(sessionId, pmc);
        VagabondLogger.Log($"ResetProfile: player profile reset {sessionId}.");
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
            VagabondLogger.Error($"PersistProfileIfPossible failed: {ex}");
        }
    }

    public static void WipeItems(MongoId sessionId, PmcData pmc, bool wipeEquipment = false,
        bool wipeStash = false, bool forceRemoveAllMoney = false, bool keepSecureContainer = false)
    {
        var inventory = pmc.Inventory;
        if (inventory == null)
        {
            VagabondLogger.Error("Wipe Equipment: inventory was null.");
            return;
        }

        if (inventory.Items == null)
        {
            VagabondLogger.Error("Wipe Equipment: inventory items list was null.");
            return;
        }

        if (inventory.Stash == null)
        {
            VagabondLogger.Error("Wipe Equipment: Stash ID null");
            return;
        }

        var invHelper = ReflectionUtil.GetService<InventoryHelper>();
        if (invHelper == null)
        {
            VagabondLogger.Error("Wipe Equipment: Inventory Helper not found");
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
        var tradersInfo = pmc.TradersInfo;
        foreach (KeyValuePair<MongoId, TraderInfo> entry in tradersInfo)
        {
            entry.Value.Disabled = true;
            entry.Value.Unlocked = false;
        
            if (VagabondConfig.Config.PermanentTraders.Contains(entry.Key))
            {
                entry.Value.Disabled = false;
                entry.Value.Unlocked = true;
            }

            if (isNewAccount && VagabondConfig.Config.StarterTraders.Contains(entry.Key))
            {
                entry.Value.Disabled = false;
                entry.Value.Unlocked = true;
            }
        }
    }

    public static bool ShouldApplyVagabondRules(MongoId sessionId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return false;
            }

            if (VagabondConfig.Config.IgnoredProfiles.Contains(sessionId))
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

            return !(pmc.ProfileInfo.Username?.StartsWith("headless_") ?? false);
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
            VagabondLogger.Error("AddMoney: required service was null.");
            return;
        }

        var amount = VagabondConfig.Config.StartingRoubles;
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

    public static string GetCurrentRaidId(VagabondState state)
    {
        if (string.IsNullOrEmpty(state.CurrentMap))
        {
            return "";
        }

        RaidLocation currentMap = VagabondLocations.NormaliseMapName(state.CurrentMap);
        if (currentMap == RaidLocation.Nil)
        {
            return "";
        }
        
        HashSet<string> allowedMapIds = new(StringComparer.OrdinalIgnoreCase);
        RaidLocation transitMap = VagabondLocations.NormaliseMapName(state.TransitState?.ToMap);
        if (transitMap != RaidLocation.Nil)
        {
            if (VagabondLocations.Locations.TryGetValue(transitMap, out var mapIds))
            {
                foreach (var mapId in mapIds)
                {
                    allowedMapIds.Add(mapId);
                }
            }
        }
        else
        {
            if (VagabondLocations.Locations.TryGetValue(currentMap, out var mapIds))
            {
                foreach (var mapId in mapIds)
                {
                    allowedMapIds.Add(mapId);
                }
            }
        }

        return allowedMapIds.First();
    }
}