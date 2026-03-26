using HardmodeChallenge.Server.Config;
using HardmodeChallenge.Server.Definitions;
using HardmodeChallenge.Server.Models;
using HardmodeChallenge.Server.Models.Enums;
using HardmodeChallenge.Server.Services;
using HardmodeChallenge.Server.State;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Utils;

namespace HardmodeChallenge.Server.Routes;

[Injectable]
public class HardmodeRouter(
    JsonUtil jsonUtil) : StaticRouter(jsonUtil, [
    
    new RouteAction<EmptyRequestData>(
        "/hardmode-challenge/sync", (_, _, sessionID, _) =>
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
        
        if (!HardmodeService.ShouldApplyHardmodeRules(sessionId))
        {
            return response;
        }

        var pmc = HardmodeService.GetPmcProfile(sessionId);
        if (pmc == null)
        {
            return response;
        }

        var state = HardmodeState.GetState(sessionId);
        response.ChallengeActive = state.ProfileInitialized;
        response.HasEnteredFirstRaid = state.HasEnteredFirstRaid;
        response.WipeEveryRaid = HardmodeConfig._config.WipeStashOnEveryRaidEntry;
        response.WipeFirstRaid = HardmodeConfig._config.WipeStashOnFirstRaidEntry;
        response.LooseAccessToTraders = HardmodeConfig._config.PreventStarterTraderAccessAfterFirstRaidEntry && HardmodeConfig._config.StarterTraders.Count > 0;
        
        var profile = HardmodeService.GetPmcProfile(sessionId);
        var pocket = profile?.CharacterData?.PmcData?.Inventory?.Items?
            .FirstOrDefault(x => string.Equals(x.SlotId, "Pockets", StringComparison.OrdinalIgnoreCase));

        foreach (var raidName in state.CompletedRaids)
        {
            HCLocation raidNameE;
            HCLocation.TryParse(raidName, true, out raidNameE);
            if (raidNameE != HCLocation.Nil && LocationData.Locations.TryGetValue(raidNameE, out var mapIds))
            {
                response.CompletedRaids.AddRange(mapIds);
            }
        }

        return response;
    }
}