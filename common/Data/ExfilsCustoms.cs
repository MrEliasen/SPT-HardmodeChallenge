using Vagabond.Common.Definitions;
using Vagabond.Common.Interfaces;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Data;

public class ExfilsCustoms : ICustomExtilData
{
    public string MapName => _mapName;
    public List<CustomExfil> Extracts => _extracts;
    public List<CustomExfil> Transits => _transits;

    private static RaidLocation _raid = RaidLocation.Customs;
    public RaidLocation Raid => _raid;

    private static string _mapName = "bigmap";

    private static List<CustomExfil> _extracts =
    [
        new CustomExfil
        {
            Identifier = "VGB_EXT_SKIER",
            DisplayName = "Skier's Hangar",
            IsTransit = false,
            TemplateExitName = "", // only fill if you want a specific template
            EntryPoints = "",
            ExfiltrationTime = 20f,
            X = 65.594f,
            Y = 1.162f,
            Z = -174.755f,
            RotationY = 356.056f,
            Side = "Pmc"
        },
    ];

    private static List<CustomExfil> _transits =
    [
        new CustomExfil
        {
            Identifier = "VGB_CT_WD",
            IsTransit = true,
            TransitPointId = 0, // gets auto generated
            DestinationLocation = VagabondLocations.InverseLookupTable[RaidLocation.Woods].First(),
            TargetLocation = VagabondLocations.InverseLookupTable[RaidLocation.Woods].First(),
            Description = "Transit to Woods",
            ExfiltrationTime = 15f,
            ActivateAfterSeconds = 0,
            IsActive = true,
            Events = false,
            HideIfNoKey = false,
            X = 653.017f,
            Y = -0.292f,
            Z = -24.815f,
            RotationY = 258.376f
        },
    ];
}