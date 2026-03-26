using System.Reflection;
using HardmodeChallenge.Server.Config;
using HardmodeChallenge.Server.Services;
using HardmodeChallenge.Server.State;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Controllers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Match;

namespace HardmodeChallenge.Server.Patches;

public sealed class RaidJoinPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(MatchController).GetMethod(nameof(MatchController.StartLocalRaid));
    }

    [PatchPrefix]
    public static void Prefix(MongoId sessionId, StartLocalRaidRequestData request)
    {
        HandleRaidEntry(sessionId, request);
    }

    public static void HandleRaidEntry(MongoId sessionId, StartLocalRaidRequestData request)
    {
        try
        {
            if (!HardmodeService.ShouldApplyHardmodeRules(sessionId))
            {
                return;
            }

            var pmc = HardmodeService.GetPmcProfile(sessionId);
            if (pmc?.CharacterData?.PmcData == null)
            {
                HardmodeLogger.Error($"Raid-entry hook could not resolve PMC profile for {sessionId}.");
                return;
            }

            var state = HardmodeState.GetState(sessionId);
            if (!state.ProfileInitialized)
            {
                return;
            }

            var mapName = request.Location;
            if (mapName == null)
            {
                HardmodeLogger.Error($"Raid-entry error: request.Location is null");
                return;
            }

            state.RaidEntryCount += 1;
            state.HasEnteredFirstRaid = true;
            HardmodeState.SaveState(sessionId, state);

            if (state.RaidEntryCount == 1 && HardmodeConfig._config.PreventStarterTraderAccessAfterFirstRaidEntry)
            {
                HardmodeService.ApplyTraderRestrictions(pmc.CharacterData.PmcData);
            }

            if (HardmodeConfig._config.WipeStashOnEveryRaidEntry ||
                HardmodeConfig._config.WipeStashOnFirstRaidEntry && state.RaidEntryCount == 1)
            {
                HardmodeService.WipeItems(sessionId, pmc.CharacterData.PmcData, state.ChallengesCompleted, false, true,
                    HardmodeConfig._config.AlsoWipeCarriedMoneyOnFirstRaid && state.RaidEntryCount == 1);
            }

            HardmodeService.PersistProfileIfPossible(sessionId);
        }
        catch
            (Exception ex)
        {
            HardmodeLogger.Error($"HandleRaidEntry failed: {ex}");
        }
    }
}