using System.Diagnostics.CodeAnalysis;
using HardmodeChallenge.Server.Services;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Services.Mod;

namespace HardmodeChallenge.Server.State;

public sealed class HardmodeState
{
    private const string ModKey = "dev.oogabooga.spt-hardmodechallenge";
    
    public bool ProfileInitialized { get; set; }
    public bool HasEnteredFirstRaid { get; set; }
    public int RaidEntryCount { get; set; }
    public int ChallengesCompleted { get; set; }
    public bool CompletedChallenge{ get; set; }
    public required List<string> CompletedRaids { get; set; }
    public bool ResetProfile { get; set; }

    [SetsRequiredMembers]
    public HardmodeState()
    {
        CompletedRaids = new List<string>();
    }
    
    public static HardmodeState GetState(MongoId sessionId)
    {
        var profileDataService = ReflectionUtil.GetService<ProfileDataService>();
        if (profileDataService == null)
        {
            return new HardmodeState();
        }

        return profileDataService.GetProfileData<HardmodeState>(sessionId, ModKey) ?? new HardmodeState();
    }

    public static void SaveState(MongoId sessionId, HardmodeState state)
    {
        var profileDataService = ReflectionUtil.GetService<ProfileDataService>();
        profileDataService?.SaveProfileData(sessionId, ModKey, state);
    }
}