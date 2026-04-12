using System.Reflection;
using EFT;
using EFT.HealthSystem;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace Vagabond.Client.Patches;

public class ActiveHealthControllerPatch : ModulePatch
{
    public static bool EnableFallDamage = false;

    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ActiveHealthController), nameof(ActiveHealthController.HandleFall));
    }

    [PatchPrefix]
    public static bool PatchPrefix(ActiveHealthController __instance, float height, Player ___Player)
    {
        if (___Player.IsAI || height <= 0)
        {
            return true;
        }

        return EnableFallDamage;
    }
}