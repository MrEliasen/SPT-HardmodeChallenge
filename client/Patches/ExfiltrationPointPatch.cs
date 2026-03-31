using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace Vagabond.Client.Patches;

internal class ExfiltrationPointPatch : ModulePatch
{
    private const string Roubles = "5449016a4bdc2d6f028b456f";

    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ExfiltrationControllerClass),
            nameof(ExfiltrationControllerClass.InitAllExfiltrationPoints));
        //return AccessTools.Method(typeof(ExfiltrationPoint), nameof(ExfiltrationPoint.LoadSettings));
    }

    [PatchPostfix]
    static void Postfix(ExfiltrationControllerClass __instance)
    {
        if (!Vagabond.State.VagabondModeEnabled && !Vagabond.IsHeadless())
        {
            return;
        }

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

            if (IsVehicleExfil(exfil.Requirements) || IsCustomExfil(exfil.Settings))
            {
                kept.Add(exfil);
                continue;
            }

            HideExfil(exfil);
        }

        __instance.ExfiltrationPoints = kept.ToArray();
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

    private static bool IsVehicleExfil(ExfiltrationRequirement[] settings)
    {
        if (settings.Length == 0)
        {
            return false;
        }

        return settings.Any(x => x.Count > 0 && x.Id == Roubles);
    }

    private static bool IsCustomExfil(ExitTriggerSettings settings)
    {
        Vagabond.Log($"TESTING => {settings.Name}");
        return !string.IsNullOrWhiteSpace(settings?.Name)
               && Vagabond.State.CustomExfils.Values
                   .SelectMany(x => x)
                   .Any(mapDefs => mapDefs.Value.Any(def =>
                       !def.IsTransit &&
                       string.Equals(def.DisplayName, settings.Name, StringComparison.OrdinalIgnoreCase)));
    }
}