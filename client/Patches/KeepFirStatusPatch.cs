using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace Vagabond.Client.Patches;

internal class KeepFirStatusPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(Profile), nameof(Profile.SetSpawnedInSession));
    }

    [PatchPostfix]
    private static void Postfix(Profile __instance, bool value)
    {
        if (value)
        {
            return;
        }

        var firItems = Vagabond.State?.RaidFirItems;
        if (firItems == null || firItems.Count == 0)
        {
            return;
        }

        foreach (var item in __instance.Inventory.GetPlayerItems())
        {
            if (firItems.Contains(item.Id))
            {
                item.SpawnedInSession = true;
            }
        }
    }
}