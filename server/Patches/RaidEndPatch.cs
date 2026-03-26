using System.Reflection;
using HardmodeChallenge.Server.Config;
using HardmodeChallenge.Server.Definitions;
using HardmodeChallenge.Server.Services;
using HardmodeChallenge.Server.State;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Services;

namespace HardmodeChallenge.Server.Patches;

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
            if (!HardmodeService.ShouldApplyHardmodeRules(sessionId))
            {
                return;
            }

            if (isDead)
            {
                return;
            }

            var state = HardmodeState.GetState(sessionId);
            if (!state.ProfileInitialized)
            {
                return;
            }

            if (!isTransfer && !request.Results.TookCarExtract([]))
            {
                return;
            }

            if (isTransfer)
            {
                // add the map to the list if they have not already been there
                if (!HardmodeService.IsMapCompleted(state.CompletedRaids, locationName))
                {
                    state.CompletedRaids.Add(locationName);
                    HardmodeState.SaveState(sessionId, state);
                }
            }

            if (request.Results.TookCarExtract([]))
            {
                if (HardmodeService.HasCompletedAllMaps(state.CompletedRaids) && !state.CompletedChallenge)
                {
                    state.ChallengesCompleted++;
                    state.ResetProfile = HardmodeConfig._config.ResetProfileOnWin;
                    state.CompletedChallenge = true;
                    HardmodeState.SaveState(sessionId, state);
                    MailerService.SendMail(sessionId, Messages.CompletedChallenge());
                    return;
                }
            }

            if (!state.CompletedChallenge)
            {
                MailerService.SendMail(sessionId,
                    "Hardmode Challenge Progress:\n" + Messages.MapProgression(state.CompletedRaids) +
                    $"\n{Messages.Rules()}");
            }
        }
        catch (Exception ex)
        {
            HardmodeLogger.Error($"HandleRaidEnd failed: {ex}");
        }
    }
}