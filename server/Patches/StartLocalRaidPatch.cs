using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Services;
using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;
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
            VagabondLogger.Success($"Raid Owner SessionId: {serverOwnerSessionId}");
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

            if (string.IsNullOrWhiteSpace(request.Location) || LocationData.NormaliseMapName(request.Location) !=
                LocationData.NormaliseMapName(transitState.ToMap))
            {
                return;
            }

            var forcedSpawn = GetSpawnLocation(
                transitState,
                __result
            );

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

    private static ManualSpawnPoint? GetSpawnLocation(
        TransitState transitState,
        StartLocalRaidResponseData response)
    {
        var template = response.LocationLoot?.SpawnPointParams?
            .FirstOrDefault(sp =>
                (sp.Categories?.Any(c => string.Equals(c, "Player", StringComparison.OrdinalIgnoreCase)) ?? false) &&
                (sp.Sides?.Any(s => string.Equals(s, "Pmc", StringComparison.OrdinalIgnoreCase)) ?? false));

        if (template == null)
        {
            return null;
        }

        var from = LocationData.NormaliseMapName(transitState.FromMap);
        var to = LocationData.NormaliseMapName(transitState.ToMap);

        return (from, to) switch
        {
            // Customs
            (RaidLocation.Interchange, RaidLocation.Customs) => new ManualSpawnPoint
                { X = -338.961f, Y = 0.793f, Z = -194.769f, Rotation = 30.629f },
            (RaidLocation.Shoreline, RaidLocation.Customs) => new ManualSpawnPoint
                { X = 9.094f, Y = -1.048f, Z = 126.674f, Rotation = 166.027f },
            (RaidLocation.Reserve, RaidLocation.Customs) => new ManualSpawnPoint
                { X = 650.311f, Y = 0.39f, Z = 116.193f, Rotation = 196.06f },
            (RaidLocation.Factory, RaidLocation.Customs) => new ManualSpawnPoint
                { X = 353.939f, Y = 1.123f, Z = -189.197f, Rotation = 3.389f },
            (RaidLocation.Woods, RaidLocation.Customs) => new ManualSpawnPoint
                { X = -4.414f, Y = 1.104f, Z = -136.337f, Rotation = 352.916f },
            // Streets
            (RaidLocation.Labs, RaidLocation.Streets) => new ManualSpawnPoint
                { X = 210.119f, Y = -8.291f, Z = 82.166f, Rotation = 88.696f },
            (RaidLocation.GroundZero, RaidLocation.Streets) => new ManualSpawnPoint
                { X = -248.599f, Y = 2.245f, Z = 98.421f, Rotation = 19.081f },
            (RaidLocation.Interchange, RaidLocation.Streets) => new ManualSpawnPoint
                { X = 288.596f, Y = 3.469f, Z = 489.124f, Rotation = 227.398f },
            // Ground Zero
            (RaidLocation.Streets, RaidLocation.GroundZero) => new ManualSpawnPoint
                { X = 229.93f, Y = 16.229f, Z = 84.142f, Rotation = 259.496f },
            // Interchange
            (RaidLocation.Streets, RaidLocation.Interchange) => new ManualSpawnPoint
                { X = 269.422f, Y = 21.401f, Z = -445.558f, Rotation = 339.283f },
            (RaidLocation.Customs, RaidLocation.Interchange) => new ManualSpawnPoint
                { X = 289.967f, Y = 21.341f, Z = 377.473f, Rotation = 274.522f },
            // Reserve
            (RaidLocation.Customs, RaidLocation.Reserve) => new ManualSpawnPoint
                { X = -200.731f, Y = -5.986f, Z = -107.305f, Rotation = 134.331f },
            (RaidLocation.Woods, RaidLocation.Reserve) => new ManualSpawnPoint
                { X = 35.423f, Y = -7.003f, Z = -221.638f, Rotation = 349.134f }, // exit to woods
            //(RaidLocation.Woods, RaidLocation.Reserve) => new ManualSpawnPoint { X = 216.246f, Y = -7.007f, Z = -176.805f, Rotation = 211.995f }, // woods exfil
            (RaidLocation.Lighthouse, RaidLocation.Reserve) => new ManualSpawnPoint
                { X = 216.246f, Y = -7.007f, Z = -176.805f, Rotation = 211.995f },
            // Woods
            (RaidLocation.Factory, RaidLocation.Woods) => new ManualSpawnPoint
                { X = -355.201f, Y = -0.268f, Z = 362.391f, Rotation = 161.997f },
            (RaidLocation.Customs, RaidLocation.Woods) => new ManualSpawnPoint
                { X = -139.908f, Y = -1.504f, Z = 417.126f, Rotation = 212.588f },
            (RaidLocation.Reserve, RaidLocation.Woods) => new ManualSpawnPoint
                { X = 252.936f, Y = -9.516f, Z = 354.375f, Rotation = 135.734f },
            (RaidLocation.Lighthouse, RaidLocation.Woods) => new ManualSpawnPoint
                { X = 498.298f, Y = -17.483f, Z = 348.645f, Rotation = 231.116f },
            // Lighthouse
            (RaidLocation.Shoreline, RaidLocation.Lighthouse) => new ManualSpawnPoint
                { X = -343.76f, Y = 8.158f, Z = -160.048f, Rotation = 109.296f },
            (RaidLocation.Reserve, RaidLocation.Lighthouse) => new ManualSpawnPoint
                { X = -313.705f, Y = 15.432f, Z = -773.452f, Rotation = 122.249f },
            (RaidLocation.Woods, RaidLocation.Lighthouse) => new ManualSpawnPoint
                { X = 104.531f, Y = 4.642f, Z = -959.373f, Rotation = 5.686f },
            // Shoreline
            (RaidLocation.Customs, RaidLocation.Shoreline) => new ManualSpawnPoint
                { X = -848.76f, Y = -42.364f, Z = 2.421f, Rotation = 29.018f },
            (RaidLocation.Lighthouse, RaidLocation.Shoreline) => new ManualSpawnPoint
                { X = 418.876f, Y = -57.395f, Z = -191.697f, Rotation = 298.845f },
            // Factory
            // Labs
            _ => null
        };
    }

    private static SpawnPointParam? GetSpawnPointTemplate(IEnumerable<SpawnPointParam>? spawnPoints,
        ManualSpawnPoint point)
    {
        return spawnPoints?
            .Where(sp =>
                (sp.Categories?.Any(c => string.Equals(c, "Player", StringComparison.OrdinalIgnoreCase)) ?? false) &&
                (sp.Sides?.Any(s => string.Equals(s, "Pmc", StringComparison.OrdinalIgnoreCase)) ?? false) &&
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
            (sp.Sides?.Any(s => string.Equals(s, "Pmc", StringComparison.OrdinalIgnoreCase)) ?? false);

        var template = GetSpawnPointTemplate(all, point);
        if (template == null)
        {
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