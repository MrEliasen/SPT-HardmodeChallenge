using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Services;
using Vagabond.Common.Data;
using Vagabond.Common.Enums;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using Vagabond.Server.Config;
using Vagabond.Server.Services;
using Vagabond.Common.Definitions;
using Vagabond.Server.State;

namespace Vagabond.Server.Patches;

public sealed class RaidEndPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(LocationLifecycleService).GetMethod(
            "HandlePostRaidPmc",
            BindingFlags.Instance | BindingFlags.NonPublic,
            Type.DefaultBinder,
            [
                typeof(MongoId),
                typeof(SptProfile),
                typeof(PmcData),
                typeof(bool),
                typeof(bool),
                typeof(bool),
                typeof(EndLocalRaidRequestData),
                typeof(string)
            ],
            null
        )!;
    }

    [PatchPrefix]
    public static void Prefix(MongoId sessionId, SptProfile fullServerProfile, bool isDead, bool isTransfer,
        EndLocalRaidRequestData request, string locationName, out HashSet<string>? __state)
    {
        __state = null;
        if (!isDead && VagabondService.ShouldApplyVagabondRules(sessionId))
        {
            var state = StateService.GetState(sessionId);
            if (state.VagabondModeEnabled && state.RaidFirItems?.Count > 0)
            {
                __state = new HashSet<string>(state.RaidFirItems);
            }
        }

        HandleRaidEnd(sessionId, fullServerProfile, isDead, isTransfer, request, locationName);
    }

    [PatchPostfix]
    public static void Postfix(MongoId sessionId, SptProfile fullServerProfile, bool isDead, bool isTransfer,
        HashSet<string>? __state)
    {
        try
        {
            if (!VagabondService.ShouldApplyVagabondRules(sessionId))
            {
                return;
            }

            var state = StateService.GetState(sessionId);
            if (!state.VagabondModeEnabled)
            {
                return;
            }

            if (isDead && VagabondConfig.Config.HealStatusEffectsOnDeath)
            {
                var bodyParts = fullServerProfile.CharacterData?.PmcData?.Health?.BodyParts;
                if (bodyParts != null)
                {
                    foreach (var part in bodyParts.Values)
                    {
                        part.Effects?.Clear();
                    }
                }
            }

            var items = fullServerProfile.CharacterData?.PmcData?.Inventory?.Items;
            if (items == null)
            {
                return;
            }

            if (__state?.Count > 0 && !isDead)
            {
                foreach (var item in items)
                {
                    if (__state.Contains(item.Id))
                    {
                        item.Upd ??= new Upd();
                        item.Upd.SpawnedInSession = true;
                    }
                }
            }

            var isOwnHideout = !string.IsNullOrEmpty(state.HideoutState?.Id) &&
                               state.LastExit == $"{HideoutService.HideoutIdPrefix}{state.HideoutState?.Id}";
            var isSharedHideout =
                (state.LastExit.IndexOf(HideoutService.HideoutIdPrefix,
                    StringComparison.OrdinalIgnoreCase) == 0) && VagabondConfig.Config.ShareHideoutExits;

            if ((isTransfer || (!isOwnHideout && !isSharedHideout)) && !isDead)
            {
                var equipmentRootId = fullServerProfile.CharacterData?.PmcData?.Inventory?.Equipment?.ToString();
                var equippedIds = GetEquipmentIds(items, equipmentRootId);
                var firIds = new HashSet<string>();
                foreach (var item in items)
                {
                    if (item.Upd?.SpawnedInSession == true && equippedIds.Contains(item.Id))
                    {
                        firIds.Add(item.Id);
                    }
                }

                state.RaidFirItems = firIds.Count > 0 ? firIds : null;
            }
            else
            {
                state.RaidFirItems = null;
            }

            StateService.SaveState(sessionId, state);
        }
        catch (Exception ex)
        {
            VagabondLogger.Error($"RaidEndPatch failed to handle fir items: {ex}");
        }
    }

    public static void HandleRaidEnd(MongoId sessionId, SptProfile profile, bool isDead, bool isTransfer,
        EndLocalRaidRequestData request, string locationName)
    {
        if (!VagabondService.ShouldApplyVagabondRules(sessionId))
        {
            return;
        }

        var state = StateService.GetState(sessionId);
        if (!state.VagabondModeEnabled)
        {
            return;
        }

        var locationMapE = VagabondLocations.NormaliseMapName(locationName);
        var locationMapStr = locationMapE.ToString();
        RaidRuntimeState.Left(sessionId);

        if (isDead)
        {
            var deathGoTo = VagabondConfig.Config.OnDeathGoTo.Trim().ToLower();
            state.ResetProfile = VagabondConfig.Config.ResetOnDeath;

            var oldCurrentMap = state.CurrentMap;
            var oldLastExit = state.LastExit;
            state.CurrentMap = "Streets";
            state.LastExit = "VGB_EXT_FENCE";

            if (string.Equals(VagabondConfig.Config.StarterFence, "lighthouse", StringComparison.OrdinalIgnoreCase))
            {
                state.CurrentMap = "Lighthouse";
                state.LastExit = "VGB_EXT_FENCE_DL";
            }

            switch (deathGoTo)
            {
                case "hideout":
                {
                    if (VagabondLocations.NormaliseMapName(state.HideoutState?.Map) != RaidLocation.Nil)
                    {
                        state.CurrentMap = VagabondLocations.NormaliseMapName(state.HideoutState?.Map).ToString();
                        state.LastExit = $"{HideoutService.HideoutIdPrefix}{state.HideoutState?.Id}";
                    }

                    break;
                }

                case "stay":
                {
                    state.CurrentMap = oldCurrentMap;
                    state.LastExit = oldLastExit;
                    break;
                }

                case "therapist":
                {
                    state.CurrentMap = "GroundZero";
                    state.LastExit = "VGB_EXT_THERAPIST";
                    break;
                }
            }

            StateService.SaveState(sessionId, state);
            return;
        }

        state.TransitState = null;
        state.CurrentMap = locationMapStr;
        state.LastExit = GetExtractIdentifier(request.Results?.ExitName, locationMapE, locationName);

        if (isTransfer)
        {
            state.TransitState = new TransitState
            {
                FromMap = locationMapStr,
                ToMap = VagabondLocations.NormaliseMapName(request.LocationTransit?.Location).ToString(),
                ExitName = state.LastExit
            };

            state.CurrentMap = state.TransitState.ToMap;
        }
        else
        {
            if (ExfilQuests.IsExfilQuest(state.LastExit, state.QuestExfils, out var traderId))
            {
                var traderLoc = HideoutService.TraderLocations.FirstOrDefault(x => x.TraderId == traderId);
                if (traderLoc != null)
                {
                    state.CurrentMap = traderLoc.Raid.ToString();
                    state.LastExit = traderLoc.ExfilIdentifier;
                }

                // Light keeper
                if (traderId == "638f541a29ffd1183d187f57")
                {
                    state.CurrentMap = nameof(RaidLocation.Lighthouse);
                    state.LastExit = "";
                }
            }

            HideoutService.UpdateTraderAccess(profile.CharacterData!.PmcData!, state);
        }

        VagabondService.PersistProfileIfPossible(sessionId);
        StateService.SaveState(sessionId, state);
    }

    private static HashSet<string> GetEquipmentIds(List<Item> items, string? equipmentRootId)
    {
        var result = new HashSet<string>();
        if (string.IsNullOrEmpty(equipmentRootId))
        {
            return result;
        }

        var childrenByParent = new Dictionary<string, List<Item>>();
        foreach (var item in items)
        {
            var parentId = item.ParentId;
            if (string.IsNullOrEmpty(parentId))
            {
                continue;
            }

            if (!childrenByParent.TryGetValue(parentId, out var itemsList))
            {
                itemsList = new List<Item>();
                childrenByParent[parentId] = itemsList;
            }

            itemsList.Add(item);
        }

        var queue = new Queue<string>();
        queue.Enqueue(equipmentRootId);

        while (queue.Count > 0)
        {
            var parentId = queue.Dequeue();
            if (!childrenByParent.TryGetValue(parentId, out var children))
            {
                continue;
            }

            foreach (var child in children)
            {
                if (result.Add(child.Id))
                {
                    queue.Enqueue(child.Id);
                }
            }
        }

        return result;
    }

    public static string GetExtractIdentifier(string? exitName, RaidLocation raid, string mapName)
    {
        if (string.IsNullOrWhiteSpace(exitName))
        {
            return string.Empty;
        }

        // its a hideout, we need the ID
        if (exitName.IndexOf(HideoutService.HideoutNamePrefix, StringComparison.OrdinalIgnoreCase) == 0)
        {
            return ExfilService.HideoutExfils[raid][mapName].FirstOrDefault(x =>
                string.Equals(x.DisplayName, exitName, StringComparison.OrdinalIgnoreCase))?.Identifier ?? string.Empty;
        }

        var match = ExfilService.CustomExfils[raid][mapName].FirstOrDefault(x =>
            string.Equals(x.Identifier, exitName, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x.DisplayName, exitName, StringComparison.OrdinalIgnoreCase));

        return match?.Identifier ?? exitName;
    }
}