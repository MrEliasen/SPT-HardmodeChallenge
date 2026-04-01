using Vagabond.Common.Definitions;
using Vagabond.Common.Interfaces;
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
    private static List<CustomExfil> _extracts = [
        new CustomExfil
        {
            Identifier = "VGB_EXT_PRAPOR",
            DisplayName = "Prapor's Warehouse",
            TemplateExitName = "Sewer River",
            IsTransit = false,
            EntryPoints = "e1_2,e2_3,e3_4,e4_5,e5_6,e6_1", // just leave them here just in case. Streets was a bastard to get to work
            ExfiltrationTime = 10f,
            X = -32.637f,
            Y = 6.608f,
            Z = -110.759f,
            RotationY = 254.802f,
            Side = "Pmc"
        },
        new CustomExfil
        {
            Identifier = "VGB_EXT_FENCE",
            DisplayName = "Fence's Hub",
            IsTransit = false,
            TemplateExitName = "",// only fill if you want a specific template
            EntryPoints = "",
            ExfiltrationTime = 20f,
            X = 74.785f,
            Y = -2.046f,
            Z = 49.224f,
            RotationY = 15.011f,
            Side = "Pmc"
        },
    ];
    private static  List<CustomExfil>_transits = [
        new CustomExfil
        {
            Identifier = "VGB_ST_WD",
            IsTransit = true,
            TransitPointId = 0,// gets auto generated
            DestinationLocation = VagabondLocations.InverseLookupTable[RaidLocation.Woods].First(),
            TargetLocation = VagabondLocations.InverseLookupTable[RaidLocation.Woods].First(),
            Description = "Transit to Woods",
            ExfiltrationTime = 15f,
            ActivateAfterSeconds = 0,
            IsActive = true,
            Events = false,
            HideIfNoKey = false,
            X = 259.298f,
            Y = -5.314f,
            Z = 76.97f,
            RotationY = 282.215f,
            ConnectedIdentifier = "VGB_WD_ST"
        },
    ];
}