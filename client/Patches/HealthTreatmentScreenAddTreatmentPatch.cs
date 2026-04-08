using System.Reflection;
using EFT.UI;
using SPT.Reflection.Patching;

namespace Vagabond.Client.Patches;

internal class HealthTreatmentScreenAddTreatmentPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(HealthTreatmentServiceView).GetMethod("method_7", BindingFlags.Public | BindingFlags.Instance);
    }

    [PatchPrefix]
    protected static bool PatchPrefix(HealthTreatmentServiceView __instance, ref bool ___bool_0)
    {
        __instance.method_10();
        ___bool_0 = false;

        return false;
    }
}