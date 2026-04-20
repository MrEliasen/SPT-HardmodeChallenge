using System.Reflection;
using EFT.UI;
using EFT.UI.Matchmaker;
using SPT.Reflection.Patching;
using TMPro;
using UnityEngine;

namespace Vagabond.Client.Patches;

// Nicked from https://github.com/dwesterwick/SPTHardcoreRules <3
internal class MatchMakerSideSelectionScreenPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(MatchMakerSideSelectionScreen).GetMethod(nameof(MatchMakerSideSelectionScreen.Update),
            BindingFlags.Public | BindingFlags.Instance);
    }

    [PatchPostfix]
    protected static void PatchPostfix(UIAnimatedToggleSpawner ____savagesButton, TMP_Text ____savageBlockMessage,
        CanvasGroup ____savageBlocker, PlayerModelView ____savageModelView)
    {
        ____savagesButton.GameObject?.SetActive(false);
        ____savageModelView.GameObject?.SetActive(false);
        ____savageBlockMessage.gameObject.SetActive(true);

        ____savageBlockMessage.SetText("Disabled (Vagabond Mod)");
        ____savageBlocker.alpha = 0.3f;
    }
}