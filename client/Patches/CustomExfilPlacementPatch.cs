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
    
    private static readonly FieldInfo TransitPointLookupField =
        AccessTools.Field(typeof(TransitControllerAbstractClass), "Dictionary_0");

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
        var locationId = gameWorld?.LocationId.ToLower();
        if (string.IsNullOrWhiteSpace(locationId))
        {
            return;
        }

        if (!LocationData.LookupTable.ContainsKey(locationId))
        {
            return;
        }
        
        var raid = LocationData.NormaliseMapName(locationId);
        if (raid == RaidLocation.Nil)
        {
            return;
        }
        
        if (!Vagabond.State.CustomExfils[raid].TryGetValue(locationId, out var definitions) || definitions == null || definitions.Count == 0)
        {
            return;
        }

        ApplyCustomExtracts(__instance, raid, definitions.Where(x => !x.IsTransit).ToList());
        ApplyCustomTransits(gameWorld, raid, definitions.Where(x => x.IsTransit).ToList());
        AppliedThisRaid = true;
    }
    
    private static void ApplyCustomExtracts(ExfiltrationControllerClass controller, RaidLocation raid, List<CustomExfilDefinition> definitions)
    {
        if (definitions.Count == 0)
        {
            return;
        }

        var pmcExfils = controller?.ExfiltrationPoints?.Where(x => x != null).ToList();
        if (pmcExfils == null || pmcExfils.Count == 0)
        {
            return;
        }

        foreach (var definition in definitions)
        {
            if (pmcExfils.Any(x => string.Equals(x.Settings?.Name, definition.DisplayName, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var template = pmcExfils.FirstOrDefault(x => string.Equals(x.Settings?.Name, definition.TemplateExitName, StringComparison.OrdinalIgnoreCase))
                           ?? pmcExfils.FirstOrDefault(x => !IsCustomExtract(x, definitions));

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

            ConfigureExtractClone(clone, template, definition, pmcExfils.Count + 1);
            pmcExfils.Add(clone);

            Vagabond.Log($"Added custom extract '{definition.DisplayName}' (identifier '{definition.Identifier}') using template '{template.Settings?.Name}'.");
        }

        controller.ExfiltrationPoints = pmcExfils.ToArray();
    }

    private static void ApplyCustomTransits(GameWorld gameWorld, RaidLocation raid, List<CustomExfilDefinition> definitions)
    {
        if (definitions.Count == 0)
        {
            return;
        }

        var transitController = gameWorld.TransitController;
        if (transitController == null)
        {
            Vagabond.LogError($"Transit controller is null on {raid}; cannot place custom transit points.");
            return;
        }

        var lookup = GetTransitLookup(transitController);
        if (lookup == null)
        {
            Vagabond.LogError("Unable to resolve transit point lookup on TransitControllerAbstractClass.");
            return;
        }

        var existingTransitPoints = LocationScene.GetAllObjects<TransitPoint>(false).Where(x => x != null).ToList();
        if (existingTransitPoints.Count == 0)
        {
            Vagabond.LogError($"No TransitPoint template exists in the {raid} scene.");
            return;
        }

        foreach (var definition in definitions)
        {
            if (!definition.TransitPointId.HasValue)
            {
                Vagabond.LogError($"Custom transit '{definition.Identifier}' is missing TransitPointId.");
                continue;
            }

            if (lookup.ContainsKey(definition.TransitPointId.Value)
                || existingTransitPoints.Any(x => x.parameters != null && x.parameters.id == definition.TransitPointId.Value))
            {
                continue;
            }

            var template = existingTransitPoints.FirstOrDefault(x => definition.TemplateTransitId.HasValue && x.parameters != null && x.parameters.id == definition.TemplateTransitId.Value)
                           ?? existingTransitPoints.FirstOrDefault();

            if (template == null)
            {
                Vagabond.LogError($"No transit template found for '{definition.Identifier}' on {raid}.");
                continue;
            }

            var cloneObject = LocationScene.Instantiate(template.gameObject);
            cloneObject.name = definition.Identifier;
            cloneObject.SetActive(true);
            cloneObject.transform.SetParent(template.transform.parent, true);
            cloneObject.transform.position = new Vector3(definition.X, definition.Y, definition.Z);
            cloneObject.transform.rotation = Quaternion.Euler(0f, definition.RotationY, 0f);

            var clone = cloneObject.GetComponent<TransitPoint>();
            if (clone == null)
            {
                Vagabond.LogError($"Cloned object for '{definition.Identifier}' does not contain TransitPoint.");
                UnityEngine.Object.Destroy(cloneObject);
                continue;
            }

            ConfigureTransitClone(clone, transitController, template, definition);
            lookup[definition.TransitPointId.Value] = clone;

            Vagabond.Log($"Added custom transit '{definition.DisplayName}' (identifier '{definition.Identifier}') to '{definition.DestinationLocation}'.");
        }
    }

    private static void ConfigureTransitClone(TransitPoint clone, TransitControllerAbstractClass controller, TransitPoint template, CustomExfilDefinition definition)
    {
        clone.Controller = controller;
        clone.Enabled = true;
        clone.IsActive = definition.IsActive;
        clone.parameters = new LocationSettingsClass.Location.TransitParameters
        {
            id = definition.TransitPointId!.Value,
            active = definition.IsActive,
            name = definition.DisplayName,
            description = string.IsNullOrWhiteSpace(definition.Description) ? definition.DisplayName : definition.Description,
            conditions = string.Empty,
            activateAfterSec = definition.ActivateAfterSeconds,
            time = (ushort)Mathf.Clamp(Mathf.RoundToInt(definition.ExfiltrationTime), 1, ushort.MaxValue),
            target = string.IsNullOrWhiteSpace(definition.TargetLocation) ? definition.DestinationLocation : definition.TargetLocation,
            location = definition.DestinationLocation,
            events = definition.Events,
            hideIfNoKey = definition.HideIfNoKey,
            eventsEnable = definition.Events
        };

        var collider = clone.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
            collider.isTrigger = true;
        }

        var templateCollider = template.GetComponent<Collider>();
        if (collider is BoxCollider box && templateCollider is BoxCollider templateBox)
        {
            box.center = templateBox.center;
            box.size = templateBox.size;
        }
    }
    
    private static void ConfigureExtractClone(ExfiltrationPoint clone, ExfiltrationPoint template, CustomExfilDefinition definition, int idOffset)
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

        // EFT matches scene exfils by Settings.Name, not by the Unity object name.
        // LoadSettings does not retarget Settings.Name, so it must be fixed manually.
        clone.Settings.Name = definition.DisplayName;
        clone.Settings.EntryPoints = settings.EntryPoints;
        clone.EligibleEntryPoints = eligibleEntryPoints;
        clone.Reusable = true;

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

        // dynamic maps fix.
        var currentEntry = Singleton<GameWorld>.Instance?.MainPlayer?.Profile?.Info?.EntryPoint;
        if (!string.IsNullOrWhiteSpace(currentEntry))
        {
            eligible.Add(currentEntry.Trim().ToLowerInvariant());
        }

        return eligible.ToArray();
    }

    private static Dictionary<int, TransitPoint>? GetTransitLookup(TransitControllerAbstractClass controller)
    {
        return TransitPointLookupField?.GetValue(controller) as Dictionary<int, TransitPoint>;
    }

    private static bool IsCustomExtract(ExfiltrationPoint exfil, List<CustomExfilDefinition> definitions)
    {
        return !string.IsNullOrWhiteSpace(exfil?.Settings?.Name)
               && definitions
                   .Where(x => !x.IsTransit)
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
