using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Interfaces;

public interface ICustomExtilData
{
    string MapName { get; }
    RaidLocation Raid { get; }
    List<CustomExfil> Extracts { get; }
    List<CustomExfil> Transits { get; }
}