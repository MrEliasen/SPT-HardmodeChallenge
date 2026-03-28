using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Services;
using Vagabond.Server.Definitions;
using Vagabond.Server.Models;
using Vagabond.Server.Models.Enums;
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
    public static void Postfix(MongoId sessionId, StartLocalRaidRequestData request,
        ref StartLocalRaidResponseData __result)
    {
        try
        {
            if (!VagabondService.ShouldApplyVagabondRules(sessionId))
            {
                return;
            }

            var state = VagabondState.GetState(sessionId);
            var transitState = state.TransitState;
            if (transitState == null)
            {
                return;
            }
            
            if (string.IsNullOrWhiteSpace(request.Location) || LocationData.NormaliseMapName(request.Location) != LocationData.NormaliseMapName(transitState.ToMap))
            {
                return;
            }

            var forcedSpawn = GetSpawnLocation(
                transitState,
                __result
            );

            if (forcedSpawn == null)
            {
                VagabondLogger.Log(
                    $"No transit spawn found for {transitState.FromMap} -> {transitState.ToMap}");

                state.TransitState = null;
                VagabondState.SaveState(sessionId, state);
                return;
            }

            VagabondLogger.Error($"Setting forced spawn {forcedSpawn}");
            __result.LocationLoot.SpawnPointParams = [forcedSpawn];
            state.TransitState = null;
            VagabondState.SaveState(sessionId, state);
        }
        catch (Exception ex)
        {
            VagabondLogger.Error($"StartLocalRaidPatch failed: {ex}");
        }
    }

    private static SpawnPointParam? GetSpawnLocation(
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

        var point = GetTransitionArrivalPoint(transitState);
        if (point == null)
        {
            return null;
        }

        return template with
        {
            Position = new XYZ
            {
                X = point.X,
                Y = point.Y,
                Z = point.Z
            },
            Rotation = point.Rotation,
            Infiltration = template.Infiltration
        };
    }

    private static ManualSpawnPoint? GetTransitionArrivalPoint(TransitState transitState)
    {
        var from = LocationData.NormaliseMapName(transitState.FromMap);
        var to = LocationData.NormaliseMapName(transitState.ToMap);
        
        VagabondLogger.Log(
            $" {from} -> {to}");

        return (from, to) switch
        {
            (RaidLocation.Interchange, RaidLocation.Customs) => new ManualSpawnPoint
            {
                X = -338.961f, Y = 0.793f, Z = -194.769f, Rotation = 30.629f
            },
            (RaidLocation.Shoreline, RaidLocation.Customs) => new ManualSpawnPoint
            {
                X = 9.094f, Y = -1.048f, Z = 126.674f, Rotation = 166.027f
            },
            (RaidLocation.Reserve, RaidLocation.Customs) => new ManualSpawnPoint
            {
                X = 650.311f, Y = 0.39f, Z = 116.193f, Rotation = 196.06f
            },
            (RaidLocation.Factory, RaidLocation.Customs) => new ManualSpawnPoint
            {
                X = 353.939f, Y = 1.123f, Z = -189.197f, Rotation = 3.389f
            },
            _ => null
        };
    }
}