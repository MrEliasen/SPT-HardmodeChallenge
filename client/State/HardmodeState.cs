using System;
using System.Collections.Generic;

namespace HardmodeChallenge.Client.State;

public sealed class HardmodeState
{
    public bool IsRefreshing { get; set; }
    public DateTime LastRefreshUtc { get; set; }
    public bool ChallengeActive { get; set; }
    public bool HasShownWarningMessage { get; set; }
    public bool HasEnteredFirstRaid { get; set; }
    public List<string> CompletedRaids { get; set; }
    public bool WipeEveryRaid{ get; set; }
    public bool WipeFirstRaid{ get; set; }
    public bool LooseAccessToTraders { get; set; }
 
}