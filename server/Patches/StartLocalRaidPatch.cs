using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Services;
using Vagabond.Common.Data;
using Vagabond.Server.Data;
using Vagabond.Server.Models;
using Vagabond.Server.Services;
using Vagabond.Server.State;

namespace Vagabond.Server.Patches;

public sealed class StartLocalRaidPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(LocationLifecycleService).GetMethod(nameof(LocationLifecycleService.StartLocalRaid))!;
    }

    [PatchPostfix]
    public static void Postfix(MongoId sessionId, StartLocalRaidRequestData request, ref StartLocalRaidResponseData __result)
    {
        try
        {
            var serverOwnerSessionId = FikaAdapter.GetRaidOwnerSessionId(sessionId);
            if (!VagabondService.ShouldApplyVagabondRules(serverOwnerSessionId))
            {
                return;
            }

            var state = VagabondState.GetState(serverOwnerSessionId);
            var transitState = state.TransitState;
            if (transitState == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(request.Location) || VagabondLocations.NormaliseMapName(request.Location) !=
                VagabondLocations.NormaliseMapName(transitState.ToMap))
            {
                return;
            }

            var forcedSpawn = StaticMapTransitions.GetSpawnLocation(transitState);
            if (forcedSpawn == null)
            {
                VagabondLogger.Log($"Did not find a PMC spawn template to clone from");
            }
            else
            {
                ApplyForcedSpawn(__result, forcedSpawn);
            }

            VagabondState.SaveState(serverOwnerSessionId, state);
        }
        catch (Exception ex)
        {
            VagabondLogger.Error($"StartLocalRaidPatch failed: {ex}");
        }
    }

    private static SpawnPointParam? GetSpawnPointTemplate(IEnumerable<SpawnPointParam>? spawnPoints,
        ManualSpawnPoint point)
    {
        return spawnPoints?
            .Where(sp =>
                (sp.Categories?.Any(c => string.Equals(c, "Player", StringComparison.OrdinalIgnoreCase)) ?? false) &&
                (sp.Sides?.Any(s => string.Equals(s, "All", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "Pmc", StringComparison.OrdinalIgnoreCase)) ?? false) &&
                sp.Position != null)
            .OrderBy(sp =>
            {
                var dx = sp.Position!.X - point.X;
                var dy = sp.Position!.Y - point.Y;
                var dz = sp.Position!.Z - point.Z;
                return dx * dx + dy * dy + dz * dz;
            })
            .FirstOrDefault();
    }

    private static void ApplyForcedSpawn(StartLocalRaidResponseData response, ManualSpawnPoint point)
    {
        var all = response.LocationLoot?.SpawnPointParams?.ToList();
        if (all == null || all.Count == 0)
        {
            VagabondLogger.Error("No map spawn points found");
            return;
        }

        bool IsPmcPlayerSpawn(SpawnPointParam sp) =>
            (sp.Categories?.Any(c => string.Equals(c, "Player", StringComparison.OrdinalIgnoreCase)) ?? false) &&
            (sp.Sides?.Any(s => string.Equals(s, "All", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "Pmc", StringComparison.OrdinalIgnoreCase)) ?? false);

        var template = GetSpawnPointTemplate(all, point);
        if (template == null)
        {
            // foreach (var sp in all)
            // {
            //     VagabondLogger.Error($"Checked => {string.Join(",", sp.Categories)} && {string.Join(",", sp.Sides)}");
            // }
            
            VagabondLogger.Error("Could not find PMC player spawn template");
            return;
        }

        var forced = template with
        {
            Position = new XYZ
            {
                X = point.X,
                Y = point.Y,
                Z = point.Z
            },
            Rotation = point.Rotation
        };

        var kept = all
            .Where(sp => !IsPmcPlayerSpawn(sp))
            .ToList();

        kept.Add(forced);
        response.LocationLoot!.SpawnPointParams = kept;

        //VagabondLogger.Log($"Forced PMC player spawn applied. Original total: {all.Count}, new total: {kept.Count}, kept non-player/non-PMC entries: {kept.Count - 1}");
    }
}