using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Data;

public class ExfilsReserve : ICustomExtilData
{
    public string MapName => _mapName;
    public List<CustomExfil> Extracts => _extracts;
    public List<CustomExfil> Transits => _transits;
    
    private static RaidLocation _raid = RaidLocation.Reserve;
    public RaidLocation Raid => _raid;

    private static string _mapName = "RezervBase";
    private static List<CustomExfil> _extracts = [];
    private static  List<CustomExfil>_transits = [];
}