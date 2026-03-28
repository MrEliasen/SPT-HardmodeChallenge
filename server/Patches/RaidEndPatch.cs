using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Services;
using Vagabond.Server.Config;
using Vagabond.Server.Definitions;
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
        HandleRaidEnd(sessionId, fullServerProfile, isDead, isTransfer, request, locationName);
    }

    public static void HandleRaidEnd(MongoId sessionId, SptProfile profile, bool isDead, bool isTransfer,
        EndLocalRaidRequestData request, string locationName)
    {
        try
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
            
            if (isDead)
            {
                state.LastExitMap = state.CompletedRaids.Count > 0 ? state.CompletedRaids.First() : "";
                VagabondState.SaveState(sessionId, state);
                MailerService.SendMail(sessionId, Messages.YouDied());
                return;
            }

            if (!isTransfer && !request.Results.TookCarExtract([]))
            {
                return;
            }

            var LocationMapE = LocationData.NormaliseMapName(locationName);
            var LocationMapStr = LocationMapE.ToString();
            state.LastExitMap = LocationMapStr;

            if (isTransfer)
            {
                // add the map to the list if they have not already been there
                if (!VagabondService.IsMapCompleted(state.CompletedRaids, LocationMapE))
                {
                    state.CompletedRaids.Add(state.LastExitMap);
                }

                state.TransitState = new TransitState
                {
                    FromMap =  LocationMapStr,
                    ToMap = LocationData.NormaliseMapName(request?.LocationTransit?.Location).ToString()
                };
            }

            VagabondState.SaveState(sessionId, state);

            if (request.Results.TookCarExtract([]))
            {
                if (VagabondService.HasCompletedAllMaps(state.CompletedRaids) && !state.CompletedChallenge)
                {
                    state.ChallengesCompleted++;
                    state.ResetProfile = VagabondConfig._config.ResetProfileOnWin;
                    state.CompletedChallenge = true;
                    VagabondState.SaveState(sessionId, state);
                    MailerService.SendMail(sessionId, Messages.CompletedChallenge());
                    return;
                }
            }

            if (!state.CompletedChallenge)
            {
                MailerService.SendMail(sessionId,
                    "Vagabond Challenge Progress:\n" + Messages.MapProgression(state.CompletedRaids) +
                    $"\n{Messages.Rules()}");
            }
        }
        catch (Exception ex)
        {
            VagabondLogger.Error($"HandleRaidEnd failed: {ex}");
        }
    }
}