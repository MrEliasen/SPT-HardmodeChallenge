using System.Reflection;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;
using Vagabond.Client.Services;

namespace Vagabond.Client.Patches;

internal class ExfiltrationPointOnTriggerExitPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ExfiltrationPoint), nameof(ExfiltrationPoint.OnTriggerExit));
    }

    [PatchPostfix]
    private static void Postfix(ExfiltrationPoint __instance, Collider col)
    {
        // as soon as they leave their infil
        ActiveHealthControllerPatch.EnableFallDamage = true;
        ExfilService.ClearSpawnOverlapSuppression(__instance, col);
    }
}