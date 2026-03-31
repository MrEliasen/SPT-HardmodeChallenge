using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Services;
using Vagabond.Common.Data;
using Vagabond.Common.Models;
using Vagabond.Server.Config;
using Vagabond.Server.Services;
using Vagabond.Server.State;

namespace Vagabond.Server.Patches;

public sealed class RaidEndPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(LocationLifecycleService).GetMethod(
            "HandlePostRaidPmc",
            BindingFlags.Instance | BindingFlags.NonPublic,
            Type.DefaultBinder,
            [
                typeof(MongoId),
                typeof(SptProfile),
                typeof(PmcData),
                typeof(bool),
                typeof(bool),
                typeof(bool),
                typeof(EndLocalRaidRequestData),
                typeof(string)
            ],
            null
        )!;
    }

    [PatchPrefix]
    public static void Prefix(MongoId sessionId, SptProfile fullServerProfile, bool isDead, bool isTransfer,
        EndLocalRaidRequestData request, string locationName)
    {
        var serverOwnerSessionId = FikaAdapter.GetRaidOwnerSessionId(sessionId);
        HandleRaidEnd(serverOwnerSessionId, fullServerProfile, isDead, isTransfer, request, locationName);
    }

    public static void HandleRaidEnd(MongoId sessionId, SptProfile profile, bool isDead, bool isTransfer,
        EndLocalRaidRequestData request, string locationName)
    {
        if (!VagabondService.ShouldApplyVagabondRules(sessionId))
        {
            return;
        }

        var state = VagabondState.GetState(sessionId);
        if (!state.VagabondModeEnabled)
        {
            return;
        }
        
        var locationMapE = VagabondLocations.NormaliseMapName(locationName);
        var locationMapStr = locationMapE.ToString();
        
        state.TransitState = null;
        state.CurrentMap = locationMapStr;
        state.LastExit = request.Results?.ExitName ?? "";
        
        if (isDead)
        {
            state.ResetProfile = VagabondConfig.Config.PermaDeath;
            VagabondState.SaveState(sessionId, state);
            return;
        }
        
        if (isTransfer)
        {
            state.TransitState = new TransitState
            {
                FromMap =  locationMapStr,
                ToMap = VagabondLocations.NormaliseMapName(request.LocationTransit?.Location).ToString(),
                ExitName = request.Results?.ExitName
            };
            
            state.CurrentMap = state.TransitState.ToMap;
        }
        else
        {
            HideoutService.UpdateTraderAccess(profile.CharacterData!.PmcData!, state);
        }

        VagabondState.SaveState(sessionId, state);
    }
}