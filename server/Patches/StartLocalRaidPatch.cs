using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Services;
using Vagabond.Common.Data;
using Vagabond.Common.Enums;
using Vagabond.Server.Data;
using Vagabond.Common.Models;
using Vagabond.Server.Services;
using Vagabond.Common.Definitions;

namespace Vagabond.Server.Patches;

public sealed class StartLocalRaidPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(LocationLifecycleService).GetMethod(nameof(LocationLifecycleService.StartLocalRaid))!;
    }

    [PatchPostfix]
    public static void Postfix(MongoId sessionId, StartLocalRaidRequestData request,
        ref StartLocalRaidResponseData __result)
    {
        try
        {
            var serverOwnerSessionId = FikaAdapter.GetRaidOwnerSessionId(sessionId);
            if (!VagabondService.ShouldApplyVagabondRules(serverOwnerSessionId))
            {
                return;
            }

            var state = VagabondStateService.GetState(serverOwnerSessionId);
            var location = VagabondLocations.NormaliseMapName(request.Location);

            if (string.IsNullOrWhiteSpace(request.Location) || location == RaidLocation.Nil)
            {
                return;
            }

            var forcedSpawn = StaticMapTransitions.GetSpawnLocation(state, location);
            if (forcedSpawn != null)
            {
                ApplyForcedSpawn(__result, request.Location, forcedSpawn);
            }

            VagabondStateService.SaveState(serverOwnerSessionId, state);
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
                (sp.Sides?.Any(s =>
                    string.Equals(s, "All", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(s, "Pmc", StringComparison.OrdinalIgnoreCase)) ?? false) &&
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

    private static void ApplyForcedSpawn(StartLocalRaidResponseData response, string locationName,
        ManualSpawnPoint point)
    {
        var all = response.LocationLoot?.SpawnPointParams?.ToList();
        if (all == null || all.Count == 0)
        {
            VagabondLogger.Error("No map spawn points found");
            return;
        }

        var template = GetSpawnPointTemplate(all, point);
        if (template?.Id == null)
        {
            VagabondLogger.Error("Could not find PMC player spawn template");
            return;
        }

        var forcedSpawnId = ForcedSpawnPointIds.Build(locationName, template.Id);
        var forced = template with
        {
            Id = forcedSpawnId,
            Position = new XYZ
            {
                X = point.X,
                Y = point.Y,
                Z = point.Z
            },
            Rotation = point.Rotation
        };

        var kept = all
            .Where(sp => !ForcedSpawnPointIds.IsForcedSpawnId(sp.Id))
            .ToList();

        kept.Add(forced);
        response.LocationLoot!.SpawnPointParams = kept;

        //VagabondLogger.Log($"Forced PMC player spawn applied. Original total: {all.Count}, new total: {kept.Count}, kept non-player/non-PMC entries: {kept.Count - 1}");
    }
}