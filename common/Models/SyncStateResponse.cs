using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Models;

public class SyncStateResponse
{
    public bool PermaDeath { get; set; }
    public bool WipeFirstRaid { get; set; }
    public string CurrentMap { get; set; } = "";
    public bool NewCharacter { get; set; }
    public bool AllowPostRaidHealing { get; set; }
    public Dictionary<string, List<string>> QuestExfils { get; set; } = new();
    public Dictionary<RaidLocation, Dictionary<string, List<CustomExfil>>> CustomExfils { get; set; } = new();
}
