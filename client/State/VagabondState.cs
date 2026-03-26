using System;
using System.Collections.Generic;

namespace Vagabond.Client.State;

public sealed class VagabondState
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