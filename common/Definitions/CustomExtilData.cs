using Vagabond.Common.Enums;
using Vagabond.Common.Interfaces;

namespace Vagabond.Common.Definitions;

/// <summary>
/// implementation of <see cref="ICustomExtilData"/>, made for the API.
/// </summary>
public sealed class CustomExtilData : ICustomExtilData
{
    public string MapName { get; set; } = string.Empty;
    public RaidLocation Raid { get; set; }
    public List<CustomExfil> Extracts { get; set; } = new();
    public List<CustomExfil> Transits { get; set; } = new();
}