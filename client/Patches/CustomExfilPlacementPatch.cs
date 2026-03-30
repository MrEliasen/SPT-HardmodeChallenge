using System.Linq;
using System.Reflection;
using EFT;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;
using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;

namespace Vagabond.Client.Patches;

internal class CustomExfilPlacementPatch : ModulePatch
{
    public static bool AppliedThisRaid;

    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnGameStarted));
    }

    // [Info   :  Vagabond] [Vagabond] CustomerExfilPlacement Running on: GameWorld
    // [Info   :  Vagabond] [Vagabond] Location ID: Shoreline
    // [Info   :  Vagabond] [Vagabond] Raid is: Shoreline
    // [Info   :  Vagabond] [Vagabond] IsCustomExfil: 5704e554d2720bac5b8b4570, exit_trigger
    // [Info   :  Vagabond] [Vagabond] IsCustomExfil: 5704e554d2720bac5b8b4571, exit_var1_constant_roadtocustoms
    // [Info   :  Vagabond] [Vagabond] IsCustomExfil: 5704e554d2720bac5b8b4572, exit_var1_ROAD_AT_RAILBRIDGE
    // [Info   :  Vagabond] [Vagabond] IsCustomExfil: 5704e554d2720bac5b8b4573, exit_var2_constant_tunnel
    // [Info   :  Vagabond] [Vagabond] IsCustomExfil: 5704e554d2720bac5b8b4574, exit_var2_LIGHTHOUSE_ROAD
    // [Info   :  Vagabond] [Vagabond] IsCustomExfil: 5704e554d2720bac5b8b457a, pmc_Smugglers Trail_coop
    // [Info   :  Vagabond] [Vagabond] IsCustomExfil: 5704e554d2720bac5b8b457b, exit_trigger
    // [Info   :  Vagabond] [Vagabond] IsCustomExfil: 5704e554d2720bac5b8b457c, res_pmc_RedRebel
        
    [PatchPostfix]
    static void Postfix(GameWorld __instance)
    {
        if (AppliedThisRaid)
        {
            return;
        }

        Vagabond.Log($"CustomerExfilPlacement Running on: {__instance.name}");

        var locationId = __instance?.LocationId;
        Vagabond.Log($"Location ID: {locationId}");
        
        if (string.IsNullOrWhiteSpace(locationId))
        {
            return;
        }
        
        var raid = LocationData.NormaliseMapName(locationId);
        Vagabond.Log($"Raid is: {raid.ToString()}");
        
        if (raid == RaidLocation.Nil)
        {
            return;
        }
        
        AppliedThisRaid = true;
        
        //var scavExfils = game.ExfiltrationController.ScavExfiltrationPoints;
        //var secretExfils = game.ExfiltrationController.SecretExfiltrationPoints;
        var pmcExfils = __instance.ExfiltrationController.ExfiltrationPoints;
        var point = pmcExfils.Where(x => !IsCustomExfil(x, raid, true))?.First();
        
        foreach (var pmcExfil in pmcExfils)
        {
            if (IsCustomExfil(pmcExfil, raid))
            {
                Vagabond.Log("YES");
                
                var newExfil = LocationScene.Instantiate(point.gameObject);
                newExfil.SetActive(true);
                newExfil.transform.SetParent(point.transform.parent, true);
                
                // var exfilPointComponent = newExfil.GetComponent<ExfiltrationPoint>();
                // if (exfilPointComponent != null)
                // {
                //     GameObject.DestroyImmediate(exfilPointComponent);
                // }

                newExfil.transform.position = new Vector3(-848.76f, -42.364f, 2.421f);
                newExfil.transform.rotation = Quaternion.Euler(0f, 30f, 0f);
                var collider = newExfil.GetComponent<Collider>();
                collider.isTrigger = true;
            }
        }
    }
    
    
    private static bool IsCustomExfil(ExfiltrationPoint exfil, RaidLocation raid, bool noLog = false)
    {
        if (!noLog)
        {
            Vagabond.Log($"IsCustomExfil: {exfil.Id}, {exfil.name}");
        }

        // [Info   :  Vagabond] [Vagabond] ExitClass: 5449016a4bdc2d6f028b456f, Shorl_V-Ex
        // [Info   :  Vagabond] [Vagabond] ExitClass: , Road to Customs
        // [Info   :  Vagabond] [Vagabond] ExitClass: , Road_at_railbridge
        // [Info   :  Vagabond] [Vagabond] ExitClass: , Tunnel
        // [Info   :  Vagabond] [Vagabond] ExitClass: , Lighthouse_pass
        // [Info   :  Vagabond] [Vagabond] ExitClass: , Smugglers_Trail_coop
        // [Info   :  Vagabond] [Vagabond] ExitClass: , Pier Boat
        // [Info   :  Vagabond] [Vagabond] ExitClass: Alpinist, RedRebel_alp
        if (string.IsNullOrWhiteSpace(exfil.name))
        {
            return false;
        }

        return Vagabond.State.CustomExfils.TryGetValue(raid, out var exfils)
               && exfils.Contains(exfil.name);
    }
    
    // if (IsCustomExfil(settings))
    // {
    //     __instance.name = settings.Name;
    //     __instance.transform.position = new UnityEngine.Vector3(-848.76f, -42.364f, 2.421f);
    //     __instance.transform.rotation = UnityEngine.Quaternion.Euler(0f, 30f, 0f);
    //
    //     var collider = __instance.GetComponent<UnityEngine.Collider>();
    //     collider.isTrigger = true;
    //     return;
    // }
    

   
    // var rawSceneName = locationScene.gameObject.scene.name.ToLowerInvariant();
    //
    // if (!ProcessedScenes.Add(rawSceneName))
    // {
    //     return;
    // }
    //
    // var mapName = rawSceneName.Replace("_scripts", "");
    // var raidLocation = LocationData.NormaliseMapName(mapName);
    //
    // Vagabond.Log($"Scene '{rawSceneName}' mapped to '{raidLocation}'");
    //
    // if (!Vagabond.State.CustomExfils.TryGetValue(raidLocation, out var customExfils))
    // {
    //     Vagabond.Log($"No custom exfils for map: {raidLocation}");
    //     return;
    // }
    //
    // Vagabond.Log($"Custom exfils found: {customExfils.Count}");
    
    // Find an existing exfil to use as a template
    // var template = pmcExfils.FirstOrDefault(x => x.name.Contains("Road", System.StringComparison.OrdinalIgnoreCase));
    //
    // if (template == null)
    // {
    //     Logger.LogError("No template ExfiltrationPoint found");
    //     return;
    // }
    //
    // // Avoid duplicates if scene reloads
    // if (existingPoints.Any(x =>
    //         x != null &&
    //         x.name.Equals("TransitToCustoms", System.StringComparison.OrdinalIgnoreCase)))
    // {
    //     return;
    // }
    //
    // var clone = Object.Instantiate(template, template.transform.parent);
    // clone.name = "TransitToCustoms";
    //
    // // Set the world position for the trigger
    // clone.transform.position = new Vector3(458.0f, 12.5f, -312.0f);
    // clone.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
    //
    // // Optional: resize trigger if needed
    // var collider = clone.GetComponent<Collider>();
    // if (collider != null)
    // {
    //     collider.isTrigger = true;
    // }
    //
    // Try to retarget the runtime settings name so it matches server-side AllExtracts/Transit name
    // This field/property name may differ slightly depending on EFT/SPT version
    // var settingsField = AccessTools.Field(clone.GetType(), "_settings");
    // if (settingsField?.GetValue(clone) is LocationExitClass settings)
    // {
    //     settings.Name = "TransitToCustoms";
    //     settings.ExfiltrationTime = 20;
    //     settings.ExfiltrationTimePVE = 20;
    //     settings.PassageRequirement = EPassageRequirement.None;
    //     settings.ExfiltrationType = EExfiltrationType.Individual;
    // }
    
    // Ensure your visibility patch allows it
    //Vagabond.State.CustomExfils.Add("TransitToCustoms");
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