using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Ws;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Utils;
using Vagabond.Common.Data;
using Vagabond.Common.Models;
using Vagabond.Server.Config;
using Vagabond.Server.Models;
using Vagabond.Server.Services;
using Vagabond.Server.State;

namespace Vagabond.Server.Routes;

[Injectable]
public class VagabondRouter(
    JsonUtil jsonUtil) : StaticRouter(jsonUtil, [
    new RouteAction<EmptyRequestData>(
        "/vagabond/sync/state",
        (_, _, sessionID, _) =>
        {
            return ValueTask.FromResult(jsonUtil.Serialize(HandleSyncStateRoute(sessionID)) ??
                                        throw new NullReferenceException("Could not serialize sync response"));
        }
    ),
    new RouteAction<GetExfilDataServerRequest>(
        "/vagabond/sync/exfils",
        (_, payload, sessionID, _) =>
        {
            return ValueTask.FromResult(jsonUtil.Serialize(HandleSyncExfilRoute(sessionID, payload)) ??
                                        throw new NullReferenceException("Could not serialize sync response"));
        }
    ),
    new RouteAction<PlaceHideoutServerRequest>(
        "/vagabond/hideout/establish",
        (_, payload, sessionID, _) =>
        {
            return ValueTask.FromResult(
                jsonUtil.Serialize(HandleEstablishHideoutRoute(sessionID, payload)) ??
                throw new NullReferenceException("Could not serialize hideout response"));
        }
    ),
])
{
    private static SyncStateResponse HandleSyncStateRoute(MongoId sessionId)
    {
        var stateSessionId = FikaAdapter.GetCanonicalSessionId(sessionId);
        var response = new SyncStateResponse
        {
            CurrentMap = ""
        };

        if (!VagabondService.IsInRaid(stateSessionId))
        {
            RaidRuntimeState.Left(stateSessionId);
        }

        if (!VagabondService.ShouldApplyVagabondRules(stateSessionId))
        {
            response.CustomExfils = ExfilService.BuildCustomExfilSnapshot();
            return response;
        }

        var pmc = VagabondService.GetPmcProfile(stateSessionId);
        if (pmc == null || pmc.CharacterData?.PmcData == null)
        {
            VagabondLogger.Error($"PMC data is null {stateSessionId}");
            response.CustomExfils = ExfilService.BuildCustomExfilSnapshot();
            return response;
        }

        var state = VagabondState.GetState(stateSessionId);
        // load their hideout first time
        if (ExfilService.AddHideoutExfil(pmc.CharacterData.PmcData, state))
        {
            ExfilService.BuildCustomExfilSnapshot(true);
        }

        //VagabondLogger.Error($"Building exfils{stateSessionId}");
        response.CustomExfils = ExfilService.BuildCustomExfilSnapshot();
        response.PermaDeath = VagabondConfig.Config.PermaDeath;
        response.WipeFirstRaid = VagabondConfig.Config.WipeStashOnFirstRaidEntry;
        response.CurrentMap = VagabondService.GetCurrentRaidId(state);
        response.NewCharacter = string.IsNullOrEmpty(state.CurrentMap);

        return response;
    }

    private static SyncExfilResponse HandleSyncExfilRoute(MongoId _, GetExfilDataServerRequest payload)
    {
        var response = new SyncExfilResponse
        {
            Version = ExfilService.SnapshotCacheVersion,
        };

        if (payload.Version != ExfilService.SnapshotCacheVersion)
        {
            response.CustomExfils = ExfilService.BuildCustomExfilSnapshot();
        }

        return response;
    }

    private static PlaceHideoutResponse HandleEstablishHideoutRoute(
        MongoId sessionId,
        PlaceHideoutServerRequest payload)
    {
        var response = new PlaceHideoutResponse();
        var stateSessionId = FikaAdapter.GetCanonicalSessionId(sessionId);

        if (!VagabondService.ShouldApplyVagabondRules(stateSessionId))
        {
            return response;
        }

        var pmc = VagabondService.GetPmcProfile(stateSessionId);
        if (pmc?.CharacterData?.PmcData == null)
        {
            return response;
        }

        var state = VagabondState.GetState(stateSessionId);

        if (state.HideoutState != null && !VagabondConfig.Config.AllowHideoutRelocation)
        {
            response.Success = false;
            response.Message = $"You have already established your hideout in {state.HideoutState.Map}.";
            return response;
        }

        var mapName = !string.IsNullOrWhiteSpace(payload.LocationId)
            ? payload.LocationId
            : VagabondService.GetCurrentRaidId(state);

        if (state.HideoutState == null)
        {
            state.HideoutState = new HideoutState
            {
                // if we do not keep the same ID, any virtual stashes tied to that hideout disappear
                Id = String.Format("{0:X}", sessionId.GetHashCode()),
            };
        }

        ExfilService.RemoveHideout(state.HideoutState);

        state.HideoutState.Map = mapName;
        state.HideoutState.X = payload.X;
        state.HideoutState.Y = payload.Y;
        state.HideoutState.Z = payload.Z;
        state.HideoutState.R = payload.R;

        ExfilService.AddHideoutExfil(pmc.CharacterData.PmcData, state);
        ExfilService.BuildCustomExfilSnapshot(true);

        VagabondState.SaveState(stateSessionId, state);
        response.Success = true;
        response.CurrentRaid = mapName;
        response.MapName = mapName;
        response.Message = "Establishing hideout, please wait...";
        return response;
    }
}