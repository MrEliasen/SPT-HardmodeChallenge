using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT.Interactive;
using EFT.Interactive.SecretExfiltrations;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace Vagabond.Client.Patches;

internal class ExfiltrationPointPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ExfiltrationControllerClass),
            nameof(ExfiltrationControllerClass.InitAllExfiltrationPoints));
        //return AccessTools.Method(typeof(ExfiltrationPoint), nameof(ExfiltrationPoint.LoadSettings));
    }

    [PatchPostfix]
    static void Postfix(ExfiltrationControllerClass __instance)
    {
        if (__instance?.ExfiltrationPoints == null)
        {
            return;
        }

        var kept = new List<ExfiltrationPoint>();
        foreach (var exfil in __instance.ExfiltrationPoints.Where(x => x != null))
        {
            if (exfil.Settings == null)
            {
                HideExfil(exfil);
                continue;
            }

            if (IsCustomExfil(exfil.Settings))
            {
                kept.Add(exfil);
                continue;
            }

            HideExfil(exfil);
        }

        __instance.ExfiltrationPoints = kept.ToArray();
        
        
        foreach (var exfil in __instance.SecretExfiltrationPoints.Where(x => x != null))
        {
            HideExfil(exfil);
        }

        __instance.SecretExfiltrationPoints = [];
    }

    private static void HideExfil(ExfiltrationPoint exfil)
    {
        exfil.Reusable = false;
        exfil.Status = EExfiltrationStatus.NotPresent;
        exfil.Disable();
        exfil.DisableInteraction();

        foreach (var collider in exfil.GetComponentsInChildren<Collider>(true))
        {
            collider.enabled = false;
        }

        exfil.gameObject.SetActive(false);
    }

    private static bool IsCustomExfil(ExitTriggerSettings settings)
    {
        return !string.IsNullOrWhiteSpace(settings?.Name)
               && Vagabond.State.CustomExfils.Values
                   .SelectMany(x => x)
                   .Any(mapDefs => mapDefs.Value.Any(def =>
                       !def.IsTransit &&
                       string.Equals(def.DisplayName, settings.Name, StringComparison.OrdinalIgnoreCase)));
    }
}