using System.Reflection;
using EFT.UI;
using SPT.Reflection.Patching;
using TMPro;

namespace Vagabond.Client.Patches;

internal class HealthTreatmentScreenShowPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(HealthTreatmentServiceView).GetMethod(nameof(HealthTreatmentServiceView.Show), BindingFlags.Public | BindingFlags.Instance);
    }

    [PatchPostfix]
    protected static void PatchPostfix(DefaultUIButton ____applyButton, UpdatableToggle ____selectAllToggle, TextMeshProUGUI ____quickHealNote, TextMeshProUGUI ____costTotalField, TextMeshProUGUI ____cashInStashField)
    {
        ____applyButton.GameObject.SetActive(false);
        ____selectAllToggle.gameObject.SetActive(false);

        ____quickHealNote.SetText("Post-raid healing disabled (Vagabond mod)");

        ____costTotalField.fontSize = ____cashInStashField.fontSize;
        ____costTotalField.SetText("N/A");
    }
}
