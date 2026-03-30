using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;
using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;
using Vagabond.Common.Models;

namespace Vagabond.Client.Patches;

internal class CustomExfilPlacementPatch : ModulePatch
{
    public static bool AppliedThisRaid;

    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ExfiltrationControllerClass), nameof(ExfiltrationControllerClass.InitAllExfiltrationPoints));
    }

    [PatchPostfix]
    private static void Postfix(ExfiltrationControllerClass __instance)
    {
        if (AppliedThisRaid)
        {
            return;
        }

        var gameWorld = Singleton<GameWorld>.Instance;
        var locationId = gameWorld?.LocationId;
        if (string.IsNullOrWhiteSpace(locationId))
        {
            return;
        }

        var raid = LocationData.NormaliseMapName(locationId);
        if (raid == RaidLocation.Nil)
        {
            return;
        }

        if (!Vagabond.State.CustomExfils.TryGetValue(raid, out var definitions) || definitions == null || definitions.Count == 0)
        {
            return;
        }

        var pmcExfils = __instance.ExfiltrationPoints?.Where(x => x != null).ToList();
        if (pmcExfils == null || pmcExfils.Count == 0)
        {
            Vagabond.LogError($"No PMC exfils available to clone for {raid}.");
            return;
        }

        AppliedThisRaid = true;

        foreach (var definition in definitions)
        {
            if (pmcExfils.Any(x => string.Equals(x.Settings?.Name, definition.DisplayName, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var template = pmcExfils.FirstOrDefault(x => string.Equals(x.Settings?.Name, definition.TemplateExitName, StringComparison.OrdinalIgnoreCase))
                           ?? pmcExfils.FirstOrDefault(x => !IsCustomExfil(x));

            if (template == null)
            {
                Vagabond.LogError($"No template exfil found for '{definition.Identifier}' on {raid}.");
                continue;
            }

            var cloneObject = LocationScene.Instantiate(template.gameObject);
            cloneObject.name = definition.Identifier;
            cloneObject.SetActive(true);
            cloneObject.transform.SetParent(template.transform.parent, true);
            cloneObject.transform.position = new Vector3(definition.X, definition.Y, definition.Z);
            cloneObject.transform.rotation = Quaternion.Euler(0f, definition.RotationY, 0f);

            var clone = cloneObject.GetComponent<ExfiltrationPoint>();
            if (clone == null)
            {
                Vagabond.LogError($"Cloned object for '{definition.Identifier}' does not contain ExfiltrationPoint.");
                UnityEngine.Object.Destroy(cloneObject);
                continue;
            }

            ConfigureClone(clone, template, definition, pmcExfils.Count + 1);
            pmcExfils.Add(clone);

            var player = gameWorld?.MainPlayer;
            Vagabond.Log(
                $"Added custom exfil '{definition.DisplayName}' (identifier '{definition.Identifier}') using template '{template.Settings?.Name}'. " +
                $"active={clone.isActiveAndEnabled}, match={(player != null && clone.InfiltrationMatch(player))}, " +
                $"playerEntry='{player?.Profile?.Info?.EntryPoint}', eligible=[{string.Join(",", clone.EligibleEntryPoints)}]");
        }

        __instance.ExfiltrationPoints = pmcExfils.ToArray();
    }

    private static void ConfigureClone(ExfiltrationPoint clone, ExfiltrationPoint template, CustomExfilDefinition definition, int idOffset)
    {
        var eligibleEntryPoints = BuildEligibleEntryPoints(definition, template);
        var settings = new LocationExitClass
        {
            Name = definition.DisplayName,
            EntryPoints = string.Join(",", eligibleEntryPoints),
            ExfiltrationTime = definition.ExfiltrationTime,
            ExfiltrationType = EExfiltrationType.Individual,
            PassageRequirement = ERequirementState.None,
            PlayersCount = 0,
            Id = string.Empty,
            Count = 0,
            RequiredSlot = EFT.InventoryLogic.EquipmentSlot.FirstPrimaryWeapon,
            RequirementTip = string.Empty,
            MinTime = 0f,
            MaxTime = 0f,
            Chance = 100f,
            EventAvailable = false
        };

        clone.Requirements = Array.Empty<ExfiltrationRequirement>();
        clone.QueuedPlayers.Clear();
        clone.LoadSettings(template.Id.Add(idOffset + 1000), settings, true);

        // EFT matches scene exfils by Settings.Name, not by the object name.
        // LoadSettings does not retarget Settings.Name, so it must be set manually.
        clone.Settings.Name = definition.DisplayName;
        clone.Settings.EntryPoints = settings.EntryPoints;
        clone.EligibleEntryPoints = eligibleEntryPoints;
        clone.Reusable = false;

        var collider = clone.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
            collider.isTrigger = true;
        }

        clone.Enable();
        clone.EnableInteraction();
        clone.SetStatusLogged(EExfiltrationStatus.RegularMode, "Vagabond.CustomExfilPlacementPatch");
    }

    private static string[] BuildEligibleEntryPoints(CustomExfilDefinition definition, ExfiltrationPoint template)
    {
        var eligible = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(definition.EntryPoints))
        {
            foreach (var entryPoint in definition.EntryPoints.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                eligible.Add(entryPoint.ToLowerInvariant());
            }
        }
        else
        {
            foreach (var entryPoint in template.EligibleEntryPoints.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                eligible.Add(entryPoint.Trim().ToLowerInvariant());
            }
        }

        // DynamicMaps fix
        var currentEntry = Singleton<GameWorld>.Instance?.MainPlayer?.Profile?.Info?.EntryPoint;
        if (!string.IsNullOrWhiteSpace(currentEntry))
        {
            eligible.Add(currentEntry.Trim().ToLowerInvariant());
        }

        return eligible.ToArray();
    }

    private static bool IsCustomExfil(ExfiltrationPoint exfil)
    {
        return !string.IsNullOrWhiteSpace(exfil?.Settings?.Name)
               && Vagabond.State.CustomExfils.Values
                   .SelectMany(x => x)
                   .Any(x => string.Equals(x.DisplayName, exfil.Settings.Name, StringComparison.OrdinalIgnoreCase));
    }
}

internal class CustomExfilCleanupPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnDestroy));
    }

    [PatchPrefix]
    private static void Prefix()
    {
        CustomExfilPlacementPatch.AppliedThisRaid = false;
    }
}
