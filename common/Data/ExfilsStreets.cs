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
            Identifier = "VGB_EXT_THERAPIST",
            DisplayName = "Therapist's Clinic",
            IsTransit = false,
            EntryPoints = "",
            ExfiltrationTime = 10f,
            X = -32.637f,
            Y = 6.608f,
            Z = -110.759f,
            RotationY = 254.802f,
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
            ExfiltrationTime = 5f,
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