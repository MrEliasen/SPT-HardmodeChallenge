using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Utils;
using Vagabond.Common.Data;
using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;
using Vagabond.Common.Models;
using Vagabond.Server.Config;
using Vagabond.Server.Models;
using Vagabond.Server.Services;
using Vagabond.Server.State;

namespace Vagabond.Server.Routes;

[Injectable]
public class VagabondRouter(JsonUtil jsonUtil) : StaticRouter(jsonUtil, [
    new RouteAction<EmptyRequestData>(
        "/vagabond/sync",
        (_, _, sessionID, _) =>
        {
            return ValueTask.FromResult(jsonUtil.Serialize(HandleSyncRoute(sessionID)) ??
                                        throw new NullReferenceException("Could not serialize sync response"));
        }
    ),
    new RouteAction<PlaceHideoutServerRequest>(
        "/vagabond/place-hideout",
        (_, payload, sessionID, _) =>
        {
            return ValueTask.FromResult(jsonUtil.Serialize(HandleEstablishHideoutRoute(sessionID, payload)) ??
                                        throw new NullReferenceException("Could not serialize hideout response"));
        }
    ),
])
{
    private static SyncStateResponse HandleSyncRoute(MongoId sessionId)
    {
        var stateSessionId = FikaAdapter.GetCanonicalSessionId(sessionId);
        var response = new SyncStateResponse
        {
            CurrentMap = ""
        };

        if (!RaidRuntimeState.IsInRaid(stateSessionId))
        {
            RaidRuntimeState.Left(stateSessionId);
        }

        if (!VagabondService.ShouldApplyVagabondRules(stateSessionId))
        {
            response.CustomExfils = BuildCustomExfilSnapshot(new VagabondState());
            return response;
        }

        var pmc = VagabondService.GetPmcProfile(stateSessionId);
        if (pmc == null)
        {
            response.CustomExfils = BuildCustomExfilSnapshot(new VagabondState());
            return response;
        }

        var state = VagabondState.GetState(stateSessionId);
        response.CustomExfils = BuildCustomExfilSnapshot(state);
        response.PermaDeath = VagabondConfig.Config.PermaDeath;
        response.WipeFirstRaid = VagabondConfig.Config.WipeStashOnFirstRaidEntry;
        response.WipeFirstMoney = VagabondConfig.Config.AlsoWipeCarriedMoneyOnFirstRaid;
        response.CurrentMap = VagabondService.GetCurrentRaidId(state);
        response.NewCharacter = string.IsNullOrEmpty(state.CurrentMap);

        return response;
    }

    private static PlaceHideoutResponse HandleEstablishHideoutRoute(MongoId sessionId,
        PlaceHideoutServerRequest payload)
    {
        var response = new PlaceHideoutResponse();
        var stateSessionId = FikaAdapter.GetCanonicalSessionId(sessionId);

        if (!VagabondService.ShouldApplyVagabondRules(stateSessionId))
        {
            return response;
        }

        var pmc = VagabondService.GetPmcProfile(stateSessionId);
        if (pmc == null)
        {
            return response;
        }

        var state = VagabondState.GetState(stateSessionId);

        if (state.HideoutState != null)
        {
            response.Success = false;
            response.Message = $"You have already established your hideout in {state.HideoutState.Map}.";
            return response;
        }

        var locationId = !string.IsNullOrWhiteSpace(payload.LocationId)
            ? payload.LocationId
            : VagabondService.GetCurrentRaidId(state);

        state.HideoutState = new HideoutState
        {
            Map = locationId,
            X = payload.X,
            Y = payload.Y,
            Z = payload.Z,
            R = payload.R
        };

        VagabondState.SaveState(stateSessionId, state);
        response.Success = true;
        response.CurrentRaid = locationId;
        response.MapName = locationId;
        response.Message = "Hideout Established.";
        response.Exfil = HideoutService.GenerateHideoutExfil(state);
        return response;
    }

    private static Dictionary<RaidLocation, Dictionary<string, List<CustomExfil>>> BuildCustomExfilSnapshot(
        VagabondState state)
    {
        var snapshot = new Dictionary<RaidLocation, Dictionary<string, List<CustomExfil>>>();

        foreach (var raidEntry in ExfilService.CustomExfils)
        {
            var byMap = new Dictionary<string, List<CustomExfil>>(StringComparer.OrdinalIgnoreCase);
            foreach (var mapEntry in raidEntry.Value)
            {
                byMap[mapEntry.Key] = [.. mapEntry.Value];
            }

            snapshot[raidEntry.Key] = byMap;
        }

        var hideoutExfil = HideoutService.GenerateHideoutExfil(state);
        if (hideoutExfil == null)
        {
            return snapshot;
        }

        var explicitMap = state.HideoutState?.Map;
        if (!string.IsNullOrWhiteSpace(explicitMap) && VagabondLocations.LookupTable.TryGetValue(explicitMap, out _))
        {
            var explicitRaid = VagabondLocations.NormaliseMapName(explicitMap);
            AddDynamicExtract(snapshot, explicitRaid, explicitMap, hideoutExfil);
            return snapshot;
        }

        var raid = VagabondLocations.NormaliseMapName(state.CurrentMap);
        if (raid == RaidLocation.Nil)
        {
            return snapshot;
        }

        if (VagabondLocations.Locations.TryGetValue(raid, out var mapIds))
        {
            foreach (var mapId in mapIds)
            {
                AddDynamicExtract(snapshot, raid, mapId, hideoutExfil);
            }
        }

        return snapshot;
    }

    private static void AddDynamicExtract(
        Dictionary<RaidLocation, Dictionary<string, List<CustomExfil>>> snapshot,
        RaidLocation raid,
        string mapId,
        CustomExfil exfil)
    {
        if (!snapshot.TryGetValue(raid, out var byMap))
        {
            byMap = new Dictionary<string, List<CustomExfil>>(StringComparer.OrdinalIgnoreCase);
            snapshot[raid] = byMap;
        }

        if (!byMap.TryGetValue(mapId, out var list))
        {
            list = [];
            byMap[mapId] = list;
        }

        if (list.Any(x => string.Equals(x.Identifier, exfil.Identifier, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        list.Add(exfil);
    }
}