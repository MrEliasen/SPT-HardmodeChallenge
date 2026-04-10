using System.Reflection;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace Vagabond.Client.Patches;

internal class ExfiltrationPointOnTriggerEnterPatch: ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ExfiltrationPoint), nameof(ExfiltrationPoint.OnTriggerEnter));
    }

    [PatchPrefix]
    private static bool Prefix(ExfiltrationPoint __instance, Collider col)
    {
        return !ExfilService.ShouldSuppressSpawnOverlap(__instance, col);
    }
}