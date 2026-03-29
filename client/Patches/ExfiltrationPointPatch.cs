using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;

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
        if (IsCustomExfil(settings))
        {
            __instance.name = settings.Name;
            __instance.transform.position = new UnityEngine.Vector3(-848.76f, -42.364f, 2.421f);
            __instance.transform.rotation = UnityEngine.Quaternion.Euler(0f, 30f, 0f);

            var collider = __instance.GetComponent<UnityEngine.Collider>();
            collider.isTrigger = true;
            return;
        }
        
        if (IsVehicleExfil(settings))
        {
            return;
        }

        __instance.Reusable = false;
            __instance.Status = EExfiltrationStatus.NotPresent;
            __instance.Disable();
            __instance.DisableInteraction();
    }

    private static bool IsVehicleExfil(LocationExitClass settings)
    {
        return settings.Count > 0 && settings.Id == Roubles;
    }
   
    private static bool IsCustomExfil(LocationExitClass exfil)
    {
        Vagabond.Log($"ExitClass: {exfil.Id}, {exfil.Name}");
        if (string.IsNullOrWhiteSpace(exfil.Name))
        {
            return false;
        }

        var locationId = GetCurrentMap();
        if (locationId == RaidLocation.Nil)
        {
            return false;
        }

        return Vagabond.State.CustomExfils.TryGetValue(locationId, out var exfils)
               && exfils.Contains(exfil.Name);
    }
    
    private static RaidLocation GetCurrentMap()
    {
        var locationId = Singleton<GameWorld>.Instance?.LocationId;

        if (string.IsNullOrWhiteSpace(locationId))
        {
            return RaidLocation.Nil;
        }

        return LocationData.NormaliseMapName(locationId);
    }
}