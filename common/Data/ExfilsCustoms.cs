using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Data;

public class ExfilsCustoms : ICustomExtilData
{
    public string MapName => _mapName;
    public List<CustomExfilDefinition> Extracts => _extracts;
    public List<CustomExfilDefinition> Transits => _transits;
    
    private static RaidLocation _raid = RaidLocation.Customs;
    public RaidLocation Raid => _raid;

    private static string _mapName = "bigmap";
    private static List<CustomExfilDefinition> _extracts = [];
    private static  List<CustomExfilDefinition>_transits = [];
}