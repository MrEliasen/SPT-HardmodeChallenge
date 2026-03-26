using System.Diagnostics.CodeAnalysis;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Services.Mod;
using Vagabond.Server.Services;

namespace Vagabond.Server.State;

public sealed class VagabondState
{
    private const string ModKey = "dev.oogabooga.spt-vagabond";
    
    public bool ProfileInitialized { get; set; }
    public bool HasEnteredFirstRaid { get; set; }
    public int RaidEntryCount { get; set; }
    public int ChallengesCompleted { get; set; }
    public bool CompletedChallenge{ get; set; }
    public required List<string> CompletedRaids { get; set; }
    public bool ResetProfile { get; set; }

    [SetsRequiredMembers]
    public VagabondState()
    {
        CompletedRaids = new List<string>();
    }
    
    public static VagabondState GetState(MongoId sessionId)
    {
        var profileDataService = ReflectionUtil.GetService<ProfileDataService>();
        if (profileDataService == null)
        {
            return new VagabondState();
        }

        return profileDataService.GetProfileData<VagabondState>(sessionId, ModKey) ?? new VagabondState();
    }

    public static void SaveState(MongoId sessionId, VagabondState state)
    {
        var profileDataService = ReflectionUtil.GetService<ProfileDataService>();
        profileDataService?.SaveProfileData(sessionId, ModKey, state);
    }
}