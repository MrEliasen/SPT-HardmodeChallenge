using System.Reflection;
using HardmodeChallenge.Server.Config;
using HardmodeChallenge.Server.Definitions;
using HardmodeChallenge.Server.Services;
using HardmodeChallenge.Server.State;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Profile;

namespace HardmodeChallenge.Server.Patches;

public sealed class ProfileBootstrapPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(ProfileHelper).GetMethod(nameof(ProfileHelper.GetPmcProfile));
    }

    [PatchPostfix]
    public static void Postfix(MongoId sessionId, ref PmcData __result)
    {
        BootstrapProfile(sessionId, __result);
    }

    public static void BootstrapProfile(MongoId sessionId, PmcData pmc)
    {
        try
        {
            if (!HardmodeService.ShouldApplyHardmodeRules(sessionId))
            {
                return;
            }

            var state = HardmodeState.GetState(sessionId);
            if (!state.ProfileInitialized)
            {
                return;
            }

            if (state.ResetProfile || state.CompletedChallenge)
            {
                var completed = state.CompletedChallenge;
                state.ResetProfile = false;
                state.CompletedChallenge = false;
                HardmodeState.SaveState(sessionId, state);
                HardmodeService.ResetProfile(sessionId, pmc, true, !HardmodeConfig._config.ResetProfileOnWin);
                HardmodeService.PersistProfileIfPossible(sessionId);
                
                if (!completed)
                {
                    MailerService.SendMail(sessionId, Messages.ProfileReset(state.CompletedRaids));
                }
                return;
            }

            HardmodeService.ApplyTraderRestrictions(pmc, !state.HasEnteredFirstRaid);
        }
        catch (Exception ex)
        {
            HardmodeLogger.Error($"Profile updating failed: {ex}");
        }
    }
}