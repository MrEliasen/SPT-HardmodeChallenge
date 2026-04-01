using Vagabond.Common.Definitions;
using Vagabond.Common.Interfaces;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Data;

public class ExfilsGroundZero : ICustomExtilData
{
    public string MapName => _mapName;
    public List<CustomExfil> Extracts => _extracts;
    public List<CustomExfil> Transits => _transits;
    
    private static RaidLocation _raid = RaidLocation.GroundZero;
    public RaidLocation Raid => _raid;

    private static string _mapName = "Sandbox_high";
    private static List<CustomExfil> _extracts = [
        new CustomExfil
        {
            Identifier = "VGB_EXT_THERAPIST",
            DisplayName = "Therapist's Clinic",
            TemplateExitName = "Sewer River",
            IsTransit = false,
            EntryPoints = "e1_2,e2_3,e3_4,e4_5,e5_6,e6_1",
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
            Identifier = "VGB_GZ_IC",
            IsTransit = true,
            TransitPointId = 0,// gets auto generated
            DestinationLocation = VagabondLocations.InverseLookupTable[RaidLocation.Interchange].First(),
            TargetLocation = VagabondLocations.InverseLookupTable[RaidLocation.Interchange].First(),
            Description = "Transit to Interchange",
            ExfiltrationTime = 5f,
            ActivateAfterSeconds = 0,
            IsActive = true,
            Events = false,
            HideIfNoKey = false,
            X = 48.189f,
            Y = 22.785f,
            Z = 325.761f,
            RotationY = 191.336f,
            ConnectedIdentifier = "VGB_IC_GZ"
        },
    ];
}