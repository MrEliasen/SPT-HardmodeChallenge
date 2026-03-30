using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;
using Vagabond.Common.Models;

namespace Vagabond.Common.Data;

public class ExfilsReserve : ICustomExtilData
{
    public string MapName => _mapName;
    public List<CustomExfilDefinition> Extracts => _extracts;
    public List<CustomExfilDefinition> Transits => _transits;
    
    private static RaidLocation _raid = RaidLocation.Reserve;
    public RaidLocation Raid => _raid;

    private static string _mapName = "rezervbase";
    private static List<CustomExfilDefinition> _extracts = [];
    private static  List<CustomExfilDefinition>_transits = [];
}