using System;
using System.Collections.Generic;
using Vagabond.Common.Enums;

namespace Vagabond.Client.State;

public sealed class VagabondState
{
    public bool IsRefreshing { get; set; }
    public DateTime LastRefreshUtc { get; set; }
    public bool ChallengeActive { get; set; }
    public bool HasShownWarningMessage { get; set; }
    public bool HasEnteredFirstRaid { get; set; }
    public HashSet<string> CompletedRaids { get; set; }
    public bool WipeEveryRaid{ get; set; }
    public bool WipeFirstRaid{ get; set; }
    public bool LooseAccessToTraders { get; set; }
    public Dictionary<RaidLocation, HashSet<string>> CustomExfils { get; set; }
}