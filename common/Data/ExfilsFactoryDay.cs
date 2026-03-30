using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;
using Vagabond.Common.Models;

namespace Vagabond.Common.Data;

public class ExfilsFactoryDay : ICustomExtilData
{
    public string MapName => _mapName;
    public List<CustomExfilDefinition> Extracts => _extracts;
    public List<CustomExfilDefinition> Transits => _transits;
    
    private static RaidLocation _raid = RaidLocation.Factory;
    public RaidLocation Raid => _raid;

    private static readonly string _mapName = "factory4_day";
    private static readonly List<CustomExfilDefinition> _extracts = [];
    private static  readonly List<CustomExfilDefinition>_transits = [];
}