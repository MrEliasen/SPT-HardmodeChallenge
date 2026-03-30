using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Data;

public class ExfilsLighthouse : ICustomExtilData
{
    public string MapName => _mapName;
    public List<CustomExfilDefinition> Extracts => _extracts;
    public List<CustomExfilDefinition> Transits => _transits;
    
    private static RaidLocation _raid = RaidLocation.Lighthouse;
    public RaidLocation Raid => _raid;

    private static string _mapName = "lighthouse";
    private static List<CustomExfilDefinition> _extracts = [];
    private static  List<CustomExfilDefinition>_transits = [
        new CustomExfilDefinition
        {
            Identifier = "lighthouse_to_shoreline",
            DisplayName = "Transit To Shoreline",
            IsTransit = true,
            TransitPointId = 0,// gets auto generated
            DestinationLocation = LocationData.InverseLookupTable[RaidLocation.Shoreline].First(),
            TargetLocation = LocationData.InverseLookupTable[RaidLocation.Shoreline].First(),
            Description = "",
            ExfiltrationTime = 20f,
            ActivateAfterSeconds = 60,
            IsActive = false,
            Events = false,
            HideIfNoKey = false,
            X = -286.691f,
            Y = 5.971f,
            Z = 419.177f,
            RotationY = 110.061f
        }
    ];
}