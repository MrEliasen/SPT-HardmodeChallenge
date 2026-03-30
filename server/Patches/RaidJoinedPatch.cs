using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Controllers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Match;
using Vagabond.Server.Config;
using Vagabond.Server.Services;
using Vagabond.Server.State;

namespace Vagabond.Server.Patches;

public sealed class RaidJoinPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(MatchController).GetMethod(nameof(MatchController.StartLocalRaid));
    }

    [PatchPrefix]
    public static void Prefix(MongoId sessionId, StartLocalRaidRequestData request)
    {
        var serverOwnerSessionId = FikaAdapter.GetRaidOwnerSessionId(sessionId);
        HandleRaidEntry(serverOwnerSessionId, request);
    }

    public static void HandleRaidEntry(MongoId sessionId, StartLocalRaidRequestData request)
    {
        if (!VagabondService.ShouldApplyVagabondRules(sessionId))
        {
            return;
        }

        var pmc = VagabondService.GetPmcProfile(sessionId);
        if (pmc?.CharacterData?.PmcData == null)
        {
            VagabondLogger.Error($"Raid-entry hook could not resolve PMC profile for {sessionId}.");
            return;
        }

        var state = VagabondState.GetState(sessionId);
        if (!state.ProfileInitialized)
        {
            return;
        }

        var mapName = request.Location;
        if (mapName == null)
        {
            VagabondLogger.Error($"Raid-entry error: request.Location is null");
            return;
        }

        state.RaidEntryCount += 1;
        state.HasEnteredFirstRaid = true;
        VagabondState.SaveState(sessionId, state);

        if (state.RaidEntryCount == 1 && VagabondConfig._config.PreventStarterTraderAccessAfterFirstRaidEntry)
        {
            VagabondService.ApplyTraderRestrictions(pmc.CharacterData.PmcData);
        }

        if (VagabondConfig._config.WipeStashOnEveryRaidEntry ||
            VagabondConfig._config.WipeStashOnFirstRaidEntry && state.RaidEntryCount == 1)
        {
            VagabondService.WipeItems(sessionId, pmc.CharacterData.PmcData, state.ChallengesCompleted, false, true,
                VagabondConfig._config.AlsoWipeCarriedMoneyOnFirstRaid && state.RaidEntryCount == 1);
        }

        VagabondService.PersistProfileIfPossible(sessionId);
    }
}