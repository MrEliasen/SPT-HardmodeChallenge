using System.Linq;
using System.Reflection;
using EFT;
using EFT.Game.Spawning;
using HarmonyLib;
using SPT.Reflection.Patching;
using Vagabond.Client.Services;
using Vagabond.Common.Data;

namespace Vagabond.Client.Patches;

internal class SpawnSystemSelectSpawnPointPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(SpawnSystemClass), nameof(SpawnSystemClass.SelectSpawnPoint));
    }

    [PatchPrefix]
    private static bool Prefix(SpawnSystemClass __instance, ESpawnCategory category, EPlayerSide side,
        ref ISpawnPoint __result)
    {
        if (category != ESpawnCategory.Player)
        {
            return true;
        }

        ForcedSpawnService.Clear();

        var forcedSpawn = __instance.ISpawnPoints
            .FirstOrDefault(sp =>
                sp != null &&
                ForcedSpawnPointIds.IsForcedSpawnId(sp.Id) &&
                sp.Categories.ContainPlayerCategory() &&
                sp.IsValid(side));

        if (forcedSpawn == null)
        {
            return true;
        }

        var referenceSpawn = __instance.ISpawnPoints
            .Where(sp =>
                sp != null &&
                !ForcedSpawnPointIds.IsForcedSpawnId(sp.Id) &&
                sp.Categories.ContainPlayerCategory() &&
                sp.IsValid(side) &&
                !string.IsNullOrWhiteSpace(sp.Infiltration))
            .OrderBy(sp => (sp.Position - forcedSpawn.Position).sqrMagnitude)
            .FirstOrDefault();

        if (referenceSpawn != null)
        {
            ForcedSpawnService.SetAbpsSpawn(referenceSpawn);
        }

        __result = forcedSpawn;
        return false;
    }
}