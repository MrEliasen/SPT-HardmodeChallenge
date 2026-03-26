using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Utils;
using Vagabond.Server.Config;
using Vagabond.Server.Definitions;
using Vagabond.Server.Models;
using Vagabond.Server.Models.Enums;
using Vagabond.Server.Services;
using Vagabond.Server.State;

namespace Vagabond.Server.Routes;

[Injectable]
public class VagabondRouter(
    JsonUtil jsonUtil) : StaticRouter(jsonUtil, [
    
    new RouteAction<EmptyRequestData>(
        "/vagabond/sync", (_, _, sessionID, _) =>
        {
            return ValueTask.FromResult(jsonUtil.Serialize(HandleRoute(sessionID)) ?? throw new NullReferenceException("Could not serialize sync response"));
        }
    ),
]) {
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
        response.LooseAccessToTraders = VagabondConfig._config.PreventStarterTraderAccessAfterFirstRaidEntry && VagabondConfig._config.StarterTraders.Count > 0;
        
        var profile = VagabondService.GetPmcProfile(sessionId);
        var pocket = profile?.CharacterData?.PmcData?.Inventory?.Items?
            .FirstOrDefault(x => string.Equals(x.SlotId, "Pockets", StringComparison.OrdinalIgnoreCase));

        foreach (var raidName in state.CompletedRaids)
        {
            RaidLocations raidNameE;
            RaidLocations.TryParse(raidName, true, out raidNameE);
            if (raidNameE != RaidLocations.Nil && LocationData.Locations.TryGetValue(raidNameE, out var mapIds))
            {
                response.CompletedRaids.AddRange(mapIds);
            }
        }

        return response;
    }
}