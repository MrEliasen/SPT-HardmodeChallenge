using Vagabond.Common.Enums;
using Vagabond.Common.Definitions;

namespace Vagabond.Common;

public interface ICustomExtilData {
    string MapName { get; }
    RaidLocation Raid { get; }
    List<CustomExfil> Extracts { get; }
    List<CustomExfil> Transits { get; }
}