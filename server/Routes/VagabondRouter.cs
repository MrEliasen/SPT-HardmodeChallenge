using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Utils;
using Vagabond.Common.Models;
using Vagabond.Server.Config;
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
}