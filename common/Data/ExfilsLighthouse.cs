using Vagabond.Common.Definitions;
using Vagabond.Common.Interfaces;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Data;

public class ExfilsLighthouse : ICustomExtilData
{
    public string MapName => _mapName;
    public List<CustomExfil> Extracts => _extracts;
    public List<CustomExfil> Transits => _transits;

    private static RaidLocation _raid = RaidLocation.Lighthouse;
    public RaidLocation Raid => _raid;

    private static string _mapName = "Lighthouse";

    private static List<CustomExfil> _extracts =
    [
        new CustomExfil
        {
            Identifier = "VGB_EXT_FENCE_DL",
            DisplayName = "Fence's Hub",
            IsTransit = false,
            TemplateExitName = "", // only fill if you want a specific template
            EntryPoints = "",
            ExfiltrationTime = 20f,
            X = -122.74f,
            Y = 10.576f,
            Z = -843.164f,
            RotationY = 90.926f,
            Side = "Pmc"
        },
    ];

    private static List<CustomExfil> _transits =
    [
        new CustomExfil
        {
            Identifier = "VGB_LH_SL",
            IsTransit = true,
            TransitPointId = 0, // gets auto generated
            DestinationLocation = VagabondLocations.InverseLookupTable[RaidLocation.Shoreline].First(),
            Description = "Transit to Shoreline",
            ExfiltrationTime = 15f,
            ActivateAfterSeconds = 0,
            IsActive = true,
            Events = false,
            HideIfNoKey = false,
            X = -286.691f,
            Y = 5.971f,
            Z = 419.177f,
            RotationY = 110.061f,
            ConnectedIdentifier = "VGB_SL_LH",
        }
    ];
}