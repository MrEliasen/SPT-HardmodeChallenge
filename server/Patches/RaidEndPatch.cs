using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Services;
using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;
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
        if (!state.ProfileInitialized)
        {
            return;
        }
        
        state.TransitState = null;
        
        var LocationMapE = VagabondLocations.NormaliseMapName(locationName);
        var LocationMapStr = LocationMapE.ToString();
        state.LastExitMap = LocationMapStr;
        
        if (isDead)
        {
            state.LastExitMap = RaidLocation.Streets.ToString();
            VagabondState.SaveState(sessionId, state);
            return;
        }
        
        if (isTransfer)
        {
            state.TransitState = new TransitState
            {
                FromMap =  LocationMapStr,
                ToMap = VagabondLocations.NormaliseMapName(request?.LocationTransit?.Location).ToString(),
                ExitName = request?.Results?.ExitName
            };
        }
        
        VagabondState.SaveState(sessionId, state);
    }
}