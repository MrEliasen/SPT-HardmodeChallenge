using System.Reflection;
using EFT.UI.Matchmaker;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace Vagabond.Client.Patches;

internal class MatchMakerLocationSelectionPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        
        return AccessTools.Method(typeof(MatchMakerSelectionLocationScreen), nameof(MatchMakerSelectionLocationScreen.method_7));
    }

    [PatchPrefix]
    private static void Prefix(ref LocationSettingsClass.Location location)
    {
        if (location == null)
        {
            return;
        }

        if (!Vagabond.State.ChallengeActive)
        {
            return;
        }

        if (!Vagabond.State.ChallengeActive || Vagabond.State.CompletedRaids.Count == 0)
        {
            return;
        }

        if (Vagabond.State.CompletedRaids.Contains(location._Id))
        {
            return;
        }

        location = null;
    }
}