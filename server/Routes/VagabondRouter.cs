using shortid;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Ws;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Utils;
using Vagabond.Common.Models;
using Vagabond.Server.Config;
using Vagabond.Server.Models;
using Vagabond.Server.Services;
using Vagabond.Server.State;

namespace Vagabond.Server.Routes;

[Injectable]
public class VagabondRouter(
    JsonUtil jsonUtil,
    NotificationSendHelper notificationSendHelper,
    SaveServer saveServer) : StaticRouter(jsonUtil, [
    new RouteAction<EmptyRequestData>(
        "/vagabond/sync/state",
        (_, _, sessionID, _) =>
        {
            return ValueTask.FromResult(jsonUtil.Serialize(HandleSyncStateRoute(sessionID)) ??
                                        throw new NullReferenceException("Could not serialize sync response"));
        }
    ),
    new RouteAction<EmptyRequestData>(
        "/vagabond/sync/exfils",
        (_, _, sessionID, _) =>
        {
            return ValueTask.FromResult(jsonUtil.Serialize(HandleSyncExfilRoute(sessionID)) ??
                                        throw new NullReferenceException("Could not serialize sync response"));
        }
    ),
    new RouteAction<PlaceHideoutServerRequest>(
        "/vagabond/hideout/establish",
        (_, payload, sessionID, _) =>
        {
            return ValueTask.FromResult(jsonUtil.Serialize(HandleEstablishHideoutRoute(sessionID, payload, notificationSendHelper, saveServer)) ??
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

        if (!RaidRuntimeState.IsInRaid(stateSessionId))
        {
            RaidRuntimeState.Left(stateSessionId);
        }

        if (!VagabondService.ShouldApplyVagabondRules(stateSessionId))
        {
            response.CustomExfils = ExfilService.BuildCustomExfilSnapshot();
            return response;
        }

        var pmc = VagabondService.GetPmcProfile(stateSessionId);
        if (pmc == null || pmc?.CharacterData?.PmcData == null)
        {
            response.CustomExfils = ExfilService.BuildCustomExfilSnapshot();
            return response;
        }
        
        var state = VagabondState.GetState(stateSessionId);
        // load their hideout first time
        ExfilService.AddHideoutExfil(pmc.CharacterData.PmcData, state);

        response.CustomExfils = ExfilService.BuildCustomExfilSnapshot();
        response.PermaDeath = VagabondConfig.Config.PermaDeath;
        response.WipeFirstRaid = VagabondConfig.Config.WipeStashOnFirstRaidEntry;
        response.WipeFirstMoney = VagabondConfig.Config.AlsoWipeCarriedMoneyOnFirstRaid;
        response.CurrentMap = VagabondService.GetCurrentRaidId(state);
        response.NewCharacter = string.IsNullOrEmpty(state.CurrentMap);

        return response;
    }
    private static SyncExfilResponse HandleSyncExfilRoute(MongoId sessionId)
    {
        var response = new SyncExfilResponse
        {
            CustomExfils = ExfilService.BuildCustomExfilSnapshot()
        };

        return response;
    }
    
    private static PlaceHideoutResponse HandleEstablishHideoutRoute(
        MongoId sessionId,
        PlaceHideoutServerRequest payload,
        NotificationSendHelper notificationSendHelper,
        SaveServer saveServer)
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
            Id = ShortId.Generate(new ShortIdOptions(length: 8, useNumbers: true, useSpecialCharacters: false)),
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
        response.Exfil = ExfilService.AddHideoutExfil(pmc.CharacterData.PmcData, state);

        BroadcastExfilRefresh(notificationSendHelper, saveServer);
        return response;
    }

    private static void BroadcastExfilRefresh(
        NotificationSendHelper notificationSendHelper,
        SaveServer saveServer)
    {
        var popup = new WsNotificationPopup
        {
            EventType = NotificationEventType.NotificationPopup,
            EventIdentifier = new MongoId(),
            Message = new MongoId(),
            Image = "vagabond-exfil-refresh"
        };

        foreach (var profileId in saveServer.GetProfiles().Keys)
        {
            if (!RaidRuntimeState.IsInRaid(profileId))
            {
                continue;
            }

            notificationSendHelper.SendMessage(profileId, popup);
        }
    }
}