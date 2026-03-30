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
        VagabondLogger.Log($"========= {serverOwnerSessionId} ============");
        HandleRaidEnd(serverOwnerSessionId, fullServerProfile, isDead, isTransfer, request, locationName);
    }

    public static void HandleRaidEnd(MongoId sessionId, SptProfile profile, bool isDead, bool isTransfer,
        EndLocalRaidRequestData request, string locationName)
    {
        VagabondLogger.Log("========= A ============");
        if (!VagabondService.ShouldApplyVagabondRules(sessionId))
        {
            VagabondLogger.Log("========= B ============");
            return;
        }

        var state = VagabondState.GetState(sessionId);
        if (!state.ProfileInitialized)
        {
            VagabondLogger.Log("========= C ============");
            return;
        }
        
        state.TransitState = null;
        
        VagabondLogger.Log("========= D ============");
        var LocationMapE = LocationData.NormaliseMapName(locationName);
        var LocationMapStr = LocationMapE.ToString();
        state.LastExitMap = LocationMapStr;
        
        if (isDead)
        {
            VagabondLogger.Log("========= E ============");
            state.LastExitMap = RaidLocation.Streets.ToString();
            VagabondState.SaveState(sessionId, state);
            return;
        }
        
        if (isTransfer)
        {
            VagabondLogger.Log("========= F ============");
            state.TransitState = new TransitState
            {
                FromMap =  LocationMapStr,
                ToMap = LocationData.NormaliseMapName(request?.LocationTransit?.Location).ToString(),
                ExitName = request?.Results?.ExitName,
            };
            VagabondLogger.Log("========= G ============");
        }
        
        VagabondLogger.Log("========= H ============");
        VagabondLogger.Log($"FromMap: {state.TransitState.FromMap}");
        VagabondLogger.Log($"ToMap: {state.TransitState.ToMap}");
        VagabondLogger.Log($"ExitName: {state.TransitState.ExitName}");
        VagabondState.SaveState(sessionId, state);
    }
}