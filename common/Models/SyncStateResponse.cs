using Vagabond.Common.Enums;

namespace Vagabond.Common.Models;

public class SyncStateResponse
{
    public bool ChallengeActive { get; set; }
    public bool HasEnteredFirstRaid{ get; set; }
    public bool WipeEveryRaid{ get; set; }
    public bool WipeFirstRaid{ get; set; }
    public bool LooseAccessToTraders{ get; set; }
    public List<string> CompletedRaids { get; set; } = new();
    public Dictionary<RaidLocation, Dictionary<string, List<CustomExfilDefinition>>> CustomExfils { get; set; } = new();
}