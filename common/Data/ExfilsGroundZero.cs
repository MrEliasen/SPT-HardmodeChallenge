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
            TemplateExitName = "",
            IsTransit = false,
            EntryPoints = "",
            ExfiltrationTime = 10f,
            X = -27.104f,
            Y = 29.715f,
            Z = 91.594f,
            RotationY = 178.348f,
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
            ExfiltrationTime = 15f,
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