using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.Game.Spawning;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;
using Vagabond.Client.State;

namespace Vagabond.Client.Patches;

internal class BotSpawnConflictionPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(BotSpawner), nameof(BotSpawner.SpawnBotsInZoneOnPositions));
    }

    [PatchPrefix]
    private static void Prefix(BotSpawner __instance, List<ISpawnPoint> openedPositions, BotZone botZone, BotCreationDataClass data)
    {
        Vagabond.Log("=== SpawnDebug ===");
        Vagabond.Log($"Time.time: {Time.time:F3}");
        Vagabond.Log($"Zone: {(botZone != null ? botZone.NameZone : "<null>")}");
        Vagabond.Log($"Opened positions: {openedPositions?.Count ?? 0}");

        var gameWorld = Singleton<GameWorld>.Instance;
        if (gameWorld == null)
        {
            Vagabond.Log("GameWorld: <null>");
            return;
        }

        var player = gameWorld.MainPlayer;
        if (player == null)
        {
            Vagabond.Log("MainPlayer: <null>");
        }
        else
        {
            var pos = player.Position;
            Vagabond.Log($"ProfileId: {player.ProfileId}");
            Vagabond.Log($"Position: X={pos.x:F3}, Y={pos.y:F3}, Z={pos.z:F3}");
            var entryPoint = player.Profile?.Info?.EntryPoint ?? "<null>";
            Vagabond.Log($"EntryPoint: {entryPoint}");
            
            TransitSpawnProtectionState.Add(
                player.Position,
                radius: 35f,
                lifetimeSeconds: 180f);
        }
        
        if (openedPositions == null || openedPositions.Count == 0 || botZone == null)
        {
            return;
        }

        if (!TransitSpawnProtectionState.HasActiveAreas())
        {
            return;
        }

        var now = Time.time;
        var validAlternatives = botZone.SpawnPoints
            .Where(p => p != null)
            .Where(p => __instance.SpawnSystem.IsValidSpawn(p, data, now))
            .Where(p => !TransitSpawnProtectionState.IsProtected(p.Position))
            .OrderByDescending(p => TransitSpawnProtectionState.MinDistanceSq(p.Position))
            .ToList();

        if (validAlternatives.Count == 0)
        {
            return;
        }

        var used = new HashSet<ISpawnPoint>( openedPositions.Where(p => p != null && !TransitSpawnProtectionState.IsProtected(p.Position)));
        for (var i = 0; i < openedPositions.Count; i++)
        {
            var current = openedPositions[i];
            if (current == null)
            {
                continue;
            }

            if (!TransitSpawnProtectionState.IsProtected(current.Position))
            {
                continue;
            }

            var replacement = validAlternatives.FirstOrDefault(p => !used.Contains(p)) ?? validAlternatives[0];
            openedPositions[i] = replacement;
            used.Add(replacement);
        }
    }
}