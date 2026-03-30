using Vagabond.Common.Enums;
using Vagabond.Common.Definitions;

namespace Vagabond.Common;

public interface ICustomExtilData {
    string MapName { get; }
    RaidLocation Raid { get; }
    List<CustomExfilDefinition> Extracts { get; }
    List<CustomExfilDefinition> Transits { get; }
}