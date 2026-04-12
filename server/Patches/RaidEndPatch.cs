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
            var state = VagabondState.GetState(sessionId);
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

            var state = VagabondState.GetState(sessionId);
            if (!state.VagabondModeEnabled)
            {
                return;
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

            if (isTransfer && !isDead)
            {
                var firIds = new List<string>();
                foreach (var item in items)
                {
                    if (item.Upd?.SpawnedInSession == true)
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

            VagabondState.SaveState(sessionId, state);
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

        var state = VagabondState.GetState(sessionId);
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

            if (deathGoTo != "stay")
            {
                state.CurrentMap = "Streets";
                state.LastExit = "VGB_EXT_FENCE";

                if (string.Equals(VagabondConfig.Config.StarterFence, "lighthouse", StringComparison.OrdinalIgnoreCase))
                {
                    state.CurrentMap = "Lighthouse";
                    state.LastExit = "VGB_EXT_FENCE_DL";
                }

                if (!string.IsNullOrEmpty(state.HideoutState?.Id) && deathGoTo == "hideout")
                {
                    state.CurrentMap = state.HideoutState.Map!;
                    state.LastExit = $"{HideoutService.HideoutIdPrefix}{state.HideoutState.Id}";
                }
            }

            VagabondState.SaveState(sessionId, state);
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
                var traderLoc = HideoutService.TraderLocations.FirstOrDefault(x => x.Id == traderId);
                if (traderLoc != null)
                {
                    state.CurrentMap = traderLoc.Raid.ToString();
                    state.LastExit = traderLoc.ExitName;
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
        VagabondState.SaveState(sessionId, state);
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