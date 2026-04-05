using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Models;

public class SyncExfilResponse
{
    public int Version { get; set; }
    public Dictionary<RaidLocation, Dictionary<string, List<CustomExfil>>>? CustomExfils { get; set; }
}