using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Utils;
using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;
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
        "/vagabond/sync",
        (_, _, sessionID, _) =>
        {
            return ValueTask.FromResult(jsonUtil.Serialize(HandleRoute(sessionID)) ??
                                        throw new NullReferenceException("Could not serialize sync response"));
        }
    ),
])
{
    private static SyncStateResponse HandleRoute(MongoId sessionId)
    {
        var response = new SyncStateResponse
        {
            ChallengeActive = false,
            CompletedRaids = [],
        };

        if (!VagabondService.ShouldApplyVagabondRules(sessionId))
        {
            return response;
        }

        var pmc = VagabondService.GetPmcProfile(sessionId);
        if (pmc == null)
        {
            return response;
        }

        var state = VagabondState.GetState(sessionId);
        response.ChallengeActive = state.ProfileInitialized;
        response.HasEnteredFirstRaid = state.HasEnteredFirstRaid;
        response.WipeEveryRaid = VagabondConfig._config.WipeStashOnEveryRaidEntry;
        response.WipeFirstRaid = VagabondConfig._config.WipeStashOnFirstRaidEntry;
        response.CustomExfils = ExfilService.CustomExfils;
        response.LooseAccessToTraders = VagabondConfig._config.PreventStarterTraderAccessAfterFirstRaidEntry &&
                                        VagabondConfig._config.StarterTraders.Count > 0;
        

        if (VagabondConfig._config.RememberLastLocation)
        {
            var raidNameE = GetCurrentRaidName(state);
            if (raidNameE != RaidLocation.Nil && LocationData.Locations.TryGetValue(raidNameE, out var mapIds))
            {
                response.CompletedRaids.AddRange(mapIds);
            }

            return response;
        }

        foreach (var raidName in state.CompletedRaids)
        {
            RaidLocation raidNameE = LocationData.NormaliseMapName(raidName);
            if (raidNameE != RaidLocation.Nil && LocationData.Locations.TryGetValue(raidNameE, out var mapIds))
            {
                response.CompletedRaids.AddRange(mapIds);
            }
        }

        return response;
    }


    private static RaidLocation GetCurrentRaidName(VagabondState state)
    {
        var mapName = state.TransitState?.ToMap;

        if (string.IsNullOrWhiteSpace(mapName))
        {
            mapName = state.LastExitMap;
        }

        if (string.IsNullOrWhiteSpace(mapName))
        {
            return RaidLocation.Nil;
        }

        return LocationData.NormaliseMapName(mapName);
    }
}