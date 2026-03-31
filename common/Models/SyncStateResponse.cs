using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Models;

public class SyncStateResponse
{
    public bool VagabondModeEnabled { get; set; }
    public bool PermaDeath { get; set; }
    public bool WipeFirstRaid{ get; set; }
    public bool WipeFirstMoney { get; set; }
    public string CurrentMap { get; set; } = "";
    public Dictionary<RaidLocation, Dictionary<string, List<CustomExfil>>> CustomExfils { get; set; } = new();
}