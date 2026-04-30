using Vagabond.Common.Definitions;
using Vagabond.Common.Interfaces;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Data;

public class ExfilsInterchange : ICustomExtilData
{
    public string MapName => _mapName;
    public List<CustomExfil> Extracts => _extracts;
    public List<CustomExfil> Transits => _transits;

    private static RaidLocation _raid = RaidLocation.Interchange;
    public RaidLocation Raid => _raid;

    private static string _mapName = "Interchange";

    private static List<CustomExfil> _extracts =
    [
        new CustomExfil
        {
            Identifier = "VGB_EXT_RAGMAN",
            DisplayName = "Ragman's Outlet",
            IsTransit = false,
            TemplateExitName = "", // only fill if you want a specific template
            EntryPoints = "",
            ExfiltrationTime = 20f,
            X = -52.581f,
            Y = 21.377f,
            Z = 46.974f,
            RotationY = 36.619f,
            Side = "Pmc"
        },
    ];

    private static List<CustomExfil> _transits =
    [
        new CustomExfil
        {
            Identifier = "VGB_IC_GZ",
            IsTransit = true,
            TransitPointId = 0, // gets auto generated
            DestinationLocation = VagabondLocations.InverseLookupTable[RaidLocation.GroundZero].First(),
            Description = "Transit to Ground Zero",
            ExfiltrationTime = 15f,
            ActivateAfterSeconds = 0,
            IsActive = true,
            Events = false,
            HideIfNoKey = false,
            X = -318.782f,
            Y = 21.365f,
            Z = 268.296f,
            RotationY = 207.341f,
            ConnectedIdentifier = "VGB_GZ_IC"
        }
    ];
}