using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Data;

public class ExfilsStreets : ICustomExtilData
{
    public string MapName => _mapName;
    public List<CustomExfil> Extracts => _extracts;
    public List<CustomExfil> Transits => _transits;
    
    private static RaidLocation _raid = RaidLocation.Streets;
    public RaidLocation Raid => _raid;

    private static string _mapName = "TarkovStreets";
    private static List<CustomExfil> _extracts = [];
    private static  List<CustomExfil>_transits = [];
}