using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using Vagabond.Client.Services;

namespace Vagabond.Client.Patches;

internal class LocalGameStopPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(
            typeof(LocalGame),
            nameof(LocalGame.Stop),
            new[] { typeof(string), typeof(ExitStatus), typeof(string), typeof(float) });
    }

    [PatchPrefix]
    private static void Prefix()
    {
        ForcedSpawnService.Clear();
    }
}