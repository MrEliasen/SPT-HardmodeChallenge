using Vagabond.Common.Enums;

namespace Vagabond.Server.Definitions;

public sealed class TraderLocation
{
    public required string Id { get; set; }
    public required RaidLocation Raid { get; set; }
    public required string ExitName { get; set; }
}