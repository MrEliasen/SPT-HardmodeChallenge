using Vagabond.Common.Definitions;
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
    private static List<CustomExfil> _extracts = [];
    private static  List<CustomExfil>_transits = [
        new CustomExfil
        {
            Identifier = "VGB_LH_SL",
            IsTransit = true,
            TransitPointId = 0,// gets auto generated
            DestinationLocation = VagabondLocations.InverseLookupTable[RaidLocation.Shoreline].First(),
            TargetLocation = VagabondLocations.InverseLookupTable[RaidLocation.Shoreline].First(),
            Description = "Transit to Shoreline",
            ExfiltrationTime = 5f,
            ActivateAfterSeconds = 5,
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