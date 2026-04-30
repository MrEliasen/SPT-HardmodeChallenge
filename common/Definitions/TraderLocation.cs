using Vagabond.Common.Enums;

namespace Vagabond.Common.Definitions;

public sealed class TraderLocation
{
    /// <summary>
    /// The Trader ID
    /// </summary>
    public string TraderId { get; set; } = string.Empty;

    /// <summary>
    /// Which raid the trader is available in
    /// </summary>
    public RaidLocation Raid { get; set; }

    /// <summary>
    /// The Identifier of the exfil (in the specified raid) which needs to be used to access the trader
    /// </summary>
    public string ExfilIdentifier { get; set; } = string.Empty;
}