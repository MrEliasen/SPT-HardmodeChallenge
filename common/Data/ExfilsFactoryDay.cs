using Vagabond.Common.Definitions;
using Vagabond.Common.Interfaces;
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
    private static List<CustomExfil> _extracts = [
        new CustomExfil
        {
            Identifier = "VGB_EXT_MECHANIC",
            DisplayName = "Mechanic's Workshop",
            IsTransit = false,
            TemplateExitName = "",// only fill if you want a specific template
            EntryPoints = "",
            ExfiltrationTime = 20f,
            X = -0.048f,
            Y = -2.633f,
            Z = 56.923f,
            RotationY = 179.542f,
            Side = "Pmc"
        },
    ];
    private static  readonly List<CustomExfil> _transits = [];
}