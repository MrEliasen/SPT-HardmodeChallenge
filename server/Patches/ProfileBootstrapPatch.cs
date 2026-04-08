using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using Vagabond.Server.Services;
using Vagabond.Server.State;

namespace Vagabond.Server.Patches;

public sealed class ProfileBootstrapPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(ProfileHelper).GetMethod(nameof(ProfileHelper.GetPmcProfile))!;
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
            if (!state.VagabondModeEnabled)
            {
                return;
            }

            MigrationService.MigrateProfile(sessionId, pmc, state);

            if (state.ResetProfile)
            {
                state.ResetProfile = false;
                VagabondState.SaveState(sessionId, state);
                VagabondService.ResetProfile(sessionId, pmc);
                VagabondService.PersistProfileIfPossible(sessionId);
                VirtualStashService.ClearAllTraderStashes(sessionId);
                ExfilService.RemoveHideout(state.HideoutState);
                return;
            }

            HideoutService.UpdateTraderAccess(pmc, state);
            VagabondService.PersistProfileIfPossible(sessionId);
        }
        catch (Exception ex)
        {
            VagabondLogger.Error($"Profile updating failed: {ex}");
        }
    }
}
