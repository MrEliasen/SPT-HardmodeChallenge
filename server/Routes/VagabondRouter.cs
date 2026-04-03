using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Utils;
using Vagabond.Common.Data;
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
        var response = new SyncStateResponse();
        response.CustomExfils = ExfilService.CustomExfils;
        response.CurrentMap = "";
        // failsafe, as it is hit in main menu
        RaidRuntimeState.Left(sessionId);

        if (VagabondService.ShouldApplyVagabondRules(sessionId))
        {
            var pmc = VagabondService.GetPmcProfile(sessionId);
            if (pmc == null)
            {
                return response;
            }

            var state = VagabondState.GetState(sessionId);
            response.PermaDeath = VagabondConfig.Config.PermaDeath;
            response.WipeFirstRaid = VagabondConfig.Config.WipeStashOnFirstRaidEntry;
            response.WipeFirstMoney = VagabondConfig.Config.AlsoWipeCarriedMoneyOnFirstRaid;
            response.CurrentMap = VagabondService.GetCurrentRaidId(state);
            response.NewCharacter = string.IsNullOrEmpty(state.CurrentMap);
        }

        return response;
    }
    
    
    private static PlaceHideoutResponse HandleEstablishHideoutRoute(MongoId sessionId, PlaceHideoutServerRequest payload)
    {
        var response = new PlaceHideoutResponse();

        if (VagabondService.ShouldApplyVagabondRules(sessionId))
        {
            var pmc = VagabondService.GetPmcProfile(sessionId);
            if (pmc == null)
            {
                return response;
            }

            var state = VagabondState.GetState(sessionId);

            if (state.HideoutState !=  null)
            {
                response.Success = false;
                response.Message = $"You have already established your hideout in {state.HideoutState.Map}.";
                return response;
            }

            state.HideoutState = new HideoutState
            {
                Map = state.CurrentMap,
                X = payload.X,
                Y = payload.Y,
                Z = payload.Z,
                R = payload.R
            };

            VagabondLocations.InverseLookupTable.TryGetValue(VagabondLocations.NormaliseMapName(state.CurrentMap), out var mapId);
            
            VagabondState.SaveState(sessionId, state);
            response.Success = true;
            response.CurrentRaid = state.CurrentMap;
            response.MapName = mapId?.First();
            response.Message = $"Hideout Established.";
            response.Exfil = HideoutService.GenerateHideoutExfil(state);
        }

        return response;
    }
}