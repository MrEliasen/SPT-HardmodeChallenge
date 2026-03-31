using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Data;

public class ExfilsFactoryDay : ICustomExtilData
{
    public string MapName => _mapName;
    public List<CustomExfil> Extracts => _extracts;
    public List<CustomExfil> Transits => _transits;
    
    private static RaidLocation _raid = RaidLocation.FactoryDay;
    public RaidLocation Raid => _raid;

    private static readonly string _mapName = "factory4_day";
    private static readonly List<CustomExfil> _extracts = [];
    private static  readonly List<CustomExfil>_transits = [];
}