using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Profile;
using Vagabond.Server.Config;
using Vagabond.Server.Definitions;
using Vagabond.Server.Services;
using Vagabond.Server.State;

namespace Vagabond.Server.Patches;

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
            if (!VagabondService.ShouldApplyVagabondRules(sessionId))
            {
                return;
            }

            var state = VagabondState.GetState(sessionId);
            if (!state.ProfileInitialized)
            {
                return;
            }

            if (state.ResetProfile || state.CompletedChallenge)
            {
                var completed = state.CompletedChallenge;
                state.ResetProfile = false;
                state.CompletedChallenge = false;
                VagabondState.SaveState(sessionId, state);
                VagabondService.ResetProfile(sessionId, pmc, true, !VagabondConfig._config.ResetProfileOnWin);
                VagabondService.PersistProfileIfPossible(sessionId);
                
                if (!completed)
                {
                    MailerService.SendMail(sessionId, Messages.ProfileReset(state.CompletedRaids));
                }
                return;
            }

            VagabondService.ApplyTraderRestrictions(pmc, !state.HasEnteredFirstRaid);
        }
        catch (Exception ex)
        {
            VagabondLogger.Error($"Profile updating failed: {ex}");
        }
    }
}