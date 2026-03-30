using System;
using System.Linq;
using System.Reflection;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace Vagabond.Client.Patches;

internal class ExfiltrationPointPatch : ModulePatch
{
    private const string Roubles = "5449016a4bdc2d6f028b456f";

    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ExfiltrationPoint), nameof(ExfiltrationPoint.LoadSettings));
    }

    [PatchPostfix]
    static void Postfix(ExfiltrationPoint __instance, LocationExitClass settings)
    {
        // if (IsVehicleExfil(settings) || IsCustomExfil(settings))
        // {
        //     return;
        // }
        //
        // __instance.Reusable = false;
        // __instance.Status = EExfiltrationStatus.NotPresent;
        // __instance.Disable();
        // __instance.DisableInteraction();
    }

    private static bool IsVehicleExfil(LocationExitClass settings)
    {
        return settings.Count > 0 && settings.Id == Roubles;
    }

    private static bool IsCustomExfil(LocationExitClass settings)
    {
        return !string.IsNullOrWhiteSpace(settings?.Name)
               && Vagabond.State.CustomExfils.Values
                   .SelectMany(x => x)
                   .Any(x => string.Equals(x.DisplayName, settings.Name, StringComparison.OrdinalIgnoreCase));
    }
}
