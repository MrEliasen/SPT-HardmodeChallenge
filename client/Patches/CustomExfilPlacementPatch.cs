using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using EFT.Interactive.SecretExfiltrations;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;
using Vagabond.Common.Data;
using Vagabond.Common.Definitions;
using Vagabond.Common.Models;
using Vagabond.Common.Enums;

namespace Vagabond.Client.Patches;

internal class CustomExfilPlacementPatch : ModulePatch
{
    public static bool ExtractsAppliedThisRaid;
    public static bool TransitsAppliedThisRaid;
    public static readonly Dictionary<int, CustomExfil> CustomTransitDefinitions = new();

    private static readonly FieldInfo TransitPointLookupField =
        AccessTools.Field(typeof(TransitControllerAbstractClass), "Dictionary_0");

    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ExfiltrationControllerClass),
            nameof(ExfiltrationControllerClass.InitAllExfiltrationPoints));
    }

    [PatchPostfix]
    public static void Postfix(ExfiltrationControllerClass __instance)
    {
        if (ExtractsAppliedThisRaid && TransitsAppliedThisRaid)
        {
            return;
        }

        var gameWorld = Singleton<GameWorld>.Instance;
        var locationId = gameWorld?.LocationId;
        if (string.IsNullOrWhiteSpace(locationId))
        {
            Vagabond.Log($"null locations");
            return;
        }

        if (!VagabondLocations.LookupTable.ContainsKey(locationId))
        {
            Vagabond.Log($"Unknown location => {locationId}");
            return;
        }

        var raid = VagabondLocations.NormaliseMapName(locationId);
        if (raid == RaidLocation.Nil)
        {
            Vagabond.Log($"Unknown Raid => {locationId}");
            return;
        }

        if (!Vagabond.State.CustomExfils.TryGetValue(raid, out var exfils) || exfils == null || exfils.Count == 0)
        {
            return;
        }

        if (!exfils.TryGetValue(locationId, out var definitions) || definitions == null || definitions.Count == 0)
        {
            return;
        }

        ApplyCustomExtracts(__instance, raid, definitions.Where(x => !x.IsTransit).ToList());
        ApplyCustomTransits(gameWorld.TransitController, raid, definitions.Where(x => x.IsTransit).ToList());
        FilterExtractions(__instance);
    }

    public static void ApplyCustomExtracts(ExfiltrationControllerClass controller, RaidLocation raid,
        List<CustomExfil> definitions)
    {
        if (ExtractsAppliedThisRaid)
        {
            return;
        }

        if (definitions.Count == 0)
        {
            return;
        }

        var pmcExfils = controller?.ExfiltrationPoints?.Where(x => x != null).ToList();
        if (pmcExfils == null || pmcExfils.Count == 0)
        {
            Vagabond.Log("ApplyCustomExtracts: no PMC exfils available");
            return;
        }

        foreach (var definition in definitions)
        {
            if (pmcExfils.Any(x =>
                    string.Equals(x.Settings?.Name, definition.DisplayName, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var template = FindTemplateExfil(pmcExfils, definition, definitions);
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

            Vagabond.Log(
                $"Added custom extract '{definition.DisplayName}' (identifier '{definition.Identifier}') using template '{template.Settings?.Name}'.");
        }

        controller.ExfiltrationPoints = pmcExfils.ToArray();
        ExtractsAppliedThisRaid = true;
    }

    private static ExfiltrationPoint FindTemplateExfil(
        List<ExfiltrationPoint> pmcExfils,
        CustomExfil definition,
        List<CustomExfil> definitions)
    {
        var currentEntry = Singleton<GameWorld>.Instance?.MainPlayer?.Profile?.Info?.EntryPoint;

        bool MatchesExplicitTemplate(ExfiltrationPoint x) =>
            string.Equals(x.Settings?.Name, definition.TemplateExitName, StringComparison.OrdinalIgnoreCase);

        bool IsGoodTemplate(ExfiltrationPoint x, bool requireActiveStatus)
        {
            if (x == null || x.Settings == null)
            {
                return false;
            }

            if (IsCustomExtract(x, definitions))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(x.Settings.Name))
            {
                return false;
            }

            if (x is SharedExfiltrationPoint)
            {
                return false;
            }

            if (x.Switch != null)
            {
                return false;
            }

            if (x.Settings.ExfiltrationType != EExfiltrationType.Individual)
            {
                return false;
            }

            if (x.Requirements != null && x.Requirements.Length > 0)
            {
                return false;
            }

            if (IsVehicleTemplate(x))
            {
                return false;
            }

            if (x.Settings.Chance < 100f)
            {
                return false;
            }

            if (x.Settings.MinTime > 0f || x.Settings.MaxTime > 0f)
            {
                return false;
            }

            if (x.Settings.EventAvailable)
            {
                return false;
            }

            if (requireActiveStatus && x.Status == EExfiltrationStatus.NotPresent)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(currentEntry))
            {
                if (x.EligibleEntryPoints == null || x.EligibleEntryPoints.Length == 0)
                {
                    return false;
                }

                if (!x.EligibleEntryPoints.Any(ep =>
                        string.Equals(ep, currentEntry, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }
            }

            return true;
        }

        if (!string.IsNullOrWhiteSpace(definition.TemplateExitName))
        {
            var explicitTemplate = pmcExfils.FirstOrDefault(x =>
                MatchesExplicitTemplate(x) && IsGoodTemplate(x, requireActiveStatus: false));

            if (explicitTemplate != null)
            {
                return explicitTemplate;
            }

            Vagabond.LogError(
                $"template '{definition.TemplateExitName}' for '{definition.Identifier}' is not a safe template.");
        }

        var preferred = pmcExfils.FirstOrDefault(x => IsGoodTemplate(x, requireActiveStatus: true));
        if (preferred != null)
        {
            return preferred;
        }

        var fallback = pmcExfils.FirstOrDefault(x => IsGoodTemplate(x, requireActiveStatus: false));
        if (fallback != null)
        {
            return fallback;
        }

        return null;
    }

    public static void ApplyCustomTransits(TransitControllerAbstractClass transitController, RaidLocation raid,
        List<CustomExfil> definitions)
    {
        if (TransitsAppliedThisRaid)
        {
            return;
        }

        if (definitions.Count == 0)
        {
            return;
        }

        if (transitController == null)
        {
            Vagabond.Log($"Transit controller is null on {raid}; cannot place custom transit points, retrying...");
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
                || existingTransitPoints.Any(x =>
                    x.parameters != null && x.parameters.id == definition.TransitPointId.Value))
            {
                continue;
            }

            var template = existingTransitPoints.FirstOrDefault(x =>
                               definition.TemplateTransitId.HasValue && x.parameters != null &&
                               x.parameters.id == definition.TemplateTransitId.Value)
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
            CustomTransitDefinitions[definition.TransitPointId.Value] = definition;

            Vagabond.Log(
                $"Added custom transit '{raid}' (identifier '{definition.Identifier}') to '{definition.DestinationLocation}'.");
        }

        TransitsAppliedThisRaid = true;
    }

    private static void ConfigureTransitClone(TransitPoint clone, TransitControllerAbstractClass controller,
        TransitPoint template, CustomExfil definition)
    {
        clone.Controller = controller;
        clone.Enabled = true;
        clone.IsActive = definition.IsActive;
        clone.parameters = new LocationSettingsClass.Location.TransitParameters
        {
            id = definition.TransitPointId!.Value,
            active = definition.IsActive,
            name = definition.Identifier,
            description = string.IsNullOrWhiteSpace(definition.Description)
                ? definition.DisplayName
                : definition.Description,
            conditions = string.Empty,
            activateAfterSec = definition.ActivateAfterSeconds,
            time = (ushort)Mathf.Clamp(Mathf.RoundToInt(definition.ExfiltrationTime), 1, ushort.MaxValue),
            target = string.IsNullOrWhiteSpace(definition.TargetLocation)
                ? definition.DestinationLocation
                : definition.TargetLocation,
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

    private static void ConfigureExtractClone(ExfiltrationPoint clone, ExfiltrationPoint template,
        CustomExfil definition, int idOffset)
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

        clone.QueuedPlayers.Clear();
        clone.LoadSettings(template.Id.Add(idOffset + 1000), settings, true);
        clone.Requirements = BuildRequirements(definition, clone);

        // EFT matches scene exfils by Settings.Name, not by the Unity object name.
        // LoadSettings does not retarget Settings.Name, so it must be fixed manually.
        clone.Settings.Name = definition.DisplayName;
        clone.Settings.EntryPoints = settings.EntryPoints;
        clone.EligibleEntryPoints = eligibleEntryPoints;
        clone.Reusable = true;
        clone.Switch = null;

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

    private static string[] BuildEligibleEntryPoints(CustomExfil definition, ExfiltrationPoint template)
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

    private static ExfiltrationRequirement[] BuildRequirements(CustomExfil definition, ExfiltrationPoint point)
    {
        if (definition.Requirements == null || definition.Requirements.Count == 0)
        {
            return Array.Empty<ExfiltrationRequirement>();
        }

        var built = new List<ExfiltrationRequirement>();

        foreach (var reqDef in definition.Requirements)
        {
            var eftType = MapRequirementType(reqDef.Type);
            if (eftType == ERequirementState.None)
            {
                continue;
            }

            var req = ExfiltrationRequirement.CreateRequirement(eftType) as ExfiltrationRequirement;
            if (req == null)
            {
                Vagabond.LogError($"Unsupported requirement '{reqDef.Type}' on '{definition.Identifier}'.");
                continue;
            }

            req.Requirement = eftType;
            req.Id = reqDef.Id;
            req.Count = reqDef.Count;
            req.RequirementTip = reqDef.RequirementTip;

            if (!string.IsNullOrWhiteSpace(reqDef.RequiredSlot)
                && Enum.TryParse<EFT.InventoryLogic.EquipmentSlot>(reqDef.RequiredSlot, true, out var slot))
            {
                req.RequiredSlot = slot;
            }

            req.Start(point);
            built.Add(req);
        }

        return built.ToArray();
    }

    private static Dictionary<int, TransitPoint> GetTransitLookup(TransitControllerAbstractClass controller)
    {
        return TransitPointLookupField?.GetValue(controller) as Dictionary<int, TransitPoint>;
    }

    private static bool IsCustomExtract(ExfiltrationPoint exfil, List<CustomExfil> definitions)
    {
        return !string.IsNullOrWhiteSpace(exfil?.Settings?.Name)
               && definitions
                   .Where(x => !x.IsTransit)
                   .Any(x => string.Equals(x.DisplayName, exfil.Settings.Name, StringComparison.OrdinalIgnoreCase));
    }

    private static ERequirementState MapRequirementType(CustomExfilRequirementType type)
    {
        return type switch
        {
            CustomExfilRequirementType.HasItem => ERequirementState.HasItem,
            CustomExfilRequirementType.EmptySlot => ERequirementState.Empty,
            _ => ERequirementState.None
        };
    }

    private static void FilterExtractions(ExfiltrationControllerClass __instance)
    {
        var kept = new List<ExfiltrationPoint>();

        foreach (var exfil in __instance.ExfiltrationPoints)
        {
            if (exfil?.Settings == null)
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
        
        foreach (var secret in __instance.SecretExfiltrationPoints)
        {
            if (secret == null)
            {
                continue;
            }

            HideExfil(secret);
        }

        __instance.SecretExfiltrationPoints = Array.Empty<SecretExfiltrationPoint>();
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
        if (string.IsNullOrWhiteSpace(settings?.Name))
        {
            return false;
        }

        var locationId = Singleton<GameWorld>.Instance?.LocationId;
        if (string.IsNullOrWhiteSpace(locationId))
        {
            return false;
        }

        var raid = VagabondLocations.NormaliseMapName(locationId);
        if (raid == RaidLocation.Nil)
        {
            return false;
        }

        if (!Vagabond.State.CustomExfils.TryGetValue(raid, out var mapExfils))
        {
            return false;
        }

        if (!mapExfils.TryGetValue(locationId, out var definitions) || definitions == null)
        {
            return false;
        }

        return definitions.Any(def =>
            !def.IsTransit &&
            string.Equals(def.DisplayName, settings.Name, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsVehicleTemplate(ExfiltrationPoint point)
    {
        return string.Equals(point.Settings?.Id, Currencies.Ruble, StringComparison.OrdinalIgnoreCase);
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
        CustomExfilPlacementPatch.TransitsAppliedThisRaid = false;
        CustomExfilPlacementPatch.ExtractsAppliedThisRaid = false;
        CustomExfilPlacementPatch.CustomTransitDefinitions.Clear();
    }
}

internal class CustomTransitRetryPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.Update));
    }

    [PatchPostfix]
    private static void Postfix(GameWorld __instance)
    {
        if (CustomExfilPlacementPatch.ExtractsAppliedThisRaid && CustomExfilPlacementPatch.TransitsAppliedThisRaid)
        {
            return;
        }

        var locationId = __instance?.LocationId;
        if (string.IsNullOrWhiteSpace(locationId))
        {
            return;
        }

        if (!VagabondLocations.LookupTable.ContainsKey(locationId))
        {
            return;
        }

        var raid = VagabondLocations.NormaliseMapName(locationId);
        if (raid == RaidLocation.Nil)
        {
            return;
        }

        if (!Vagabond.State.CustomExfils.TryGetValue(raid, out var exfils) || exfils == null || exfils.Count == 0)
        {
            return;
        }

        if (!exfils.TryGetValue(locationId, out var definitions) || definitions == null || definitions.Count == 0)
        {
            return;
        }

        CustomExfilPlacementPatch.ApplyCustomExtracts(__instance.ExfiltrationController, raid,
            definitions.Where(x => !x.IsTransit).ToList());
        CustomExfilPlacementPatch.ApplyCustomTransits(__instance.TransitController, raid,
            definitions.Where(x => x.IsTransit).ToList());
    }
}