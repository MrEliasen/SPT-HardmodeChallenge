using System;
using System.Collections.Generic;
using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;

namespace Vagabond.Client.State;

public sealed class VagabondState
{
    public bool IsRefreshing { get; set; }
    public DateTime LastRefreshUtc { get; set; }
    public bool HasShownWarningMessage { get; set; }
    public bool PermaDeath { get; set; }
    public string CurrentMap { get; set; }
    public bool WipeFirstRaid { get; set; }
    public bool NewCharacter { get; set; }
    public int CustomExfilsCacheVersion = 0;
    public Dictionary<RaidLocation, Dictionary<string, List<CustomExfil>>> CustomExfils { get; set; } = new();
}