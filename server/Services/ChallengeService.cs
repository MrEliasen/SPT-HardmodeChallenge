using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Match;
using Vagabond.Common.Definitions;
using Vagabond.Server.Config;
using Vagabond.Server.Definitions;
using Vagabond.Server.State;

namespace Vagabond.Server.Services;

internal static class ChallengeService
{
    public static void WelcomePlayer(MongoId sessionId)
    {
        if (VagabondConfig._config.EnableChallenge)
        {
            MailerService.SendMail(sessionId, Messages.WelcomeChallenge());
            return;
        }
        
        MailerService.SendMail(sessionId, Messages.WelcomeOpenWorld());
    }
    
    public static void ResetNotification(MongoId sessionId)
    {
        if (VagabondConfig._config.EnableChallenge)
        {
            MailerService.SendMail(sessionId, Messages.ProfileReset([]));
            return;
        }
        
        MailerService.SendMail(sessionId, Messages.ProfileResetGeneric());
    }
    
    public static bool HandleExfil(MongoId sessionId, string locationName, VagabondState state, EndLocalRaidRequestData request, bool isDead, bool isTransfer, bool isCarExtract)
    {
        if (VagabondConfig._config.EnableChallenge)
        {
            return false;
        }

        if (isDead)
        {
            state.LastExitMap = state.CompletedRaids.Count > 0 ? state.CompletedRaids.First() : "";
            VagabondState.SaveState(sessionId, state);
            MailerService.SendMail(sessionId, Messages.YouDied());
            return true;
        }

        if (!isTransfer && !isCarExtract)
        {
            return true;
        }

        var LocationMapE = LocationData.NormaliseMapName(locationName);
        var LocationMapStr = LocationMapE.ToString();
        state.LastExitMap = LocationMapStr;

        if (isTransfer)
        {
            if (!VagabondService.IsMapCompleted(state.CompletedRaids, LocationMapE))
            {
                state.CompletedRaids.Add(state.LastExitMap);
            }

            state.TransitState = new TransitState
            {
                FromMap =  LocationMapStr,
                ToMap = LocationData.NormaliseMapName(request?.LocationTransit?.Location).ToString(),
                ExitName = request?.Results?.ExitName,
            };
        }

        VagabondState.SaveState(sessionId, state);
        
        if (isCarExtract && VagabondService.HasCompletedAllMaps(state.CompletedRaids) && !state.CompletedChallenge)
        {
            state.ChallengesCompleted++;
            state.ResetProfile = VagabondConfig._config.ResetProfileOnWin;
            state.CompletedChallenge = true;
            VagabondState.SaveState(sessionId, state);
            MailerService.SendMail(sessionId, Messages.CompletedChallenge());
            return true;
        }

        if (!state.CompletedChallenge)
        {
            MailerService.SendMail(sessionId,
                "Vagabond Challenge Progress:\n" + Messages.MapProgression(state.CompletedRaids) +
                $"\n{Messages.Rules()}");
        }
        
        return true;
    }
}