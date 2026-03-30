using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;
using Vagabond.Common.Models;

namespace Vagabond.Common.Data;

public class ExfilsLighthouse : ICustomExtilData
{
    public string MapName => _mapName;
    public List<CustomExfilDefinition> Extracts => _extracts;
    public List<CustomExfilDefinition> Transits => _transits;
    
    private static RaidLocation _raid = RaidLocation.Lighthouse;
    public RaidLocation Raid => _raid;

    private static string _mapName = "lighthouse";
    private static List<CustomExfilDefinition> _extracts = [];
    private static  List<CustomExfilDefinition>_transits = [];
}