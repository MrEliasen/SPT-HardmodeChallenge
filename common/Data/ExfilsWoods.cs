using Vagabond.Common.Definitions;
using Vagabond.Common.Interfaces;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Data;

public class ExfilsWoods : ICustomExtilData
{
    public string MapName => _mapName;
    public List<CustomExfil> Extracts => _extracts;
    public List<CustomExfil> Transits => _transits;
    
    private static RaidLocation _raid = RaidLocation.Woods;
    public RaidLocation Raid => _raid;

    private static string _mapName = "Woods";
    private static List<CustomExfil> _extracts = [];
    private static  List<CustomExfil>_transits = [
        new CustomExfil
        {
            Identifier = "VGB_WD_ST",
            IsTransit = true,
            TransitPointId = 0,// gets auto generated
            DestinationLocation = VagabondLocations.InverseLookupTable[RaidLocation.Streets].First(),
            TargetLocation = VagabondLocations.InverseLookupTable[RaidLocation.Streets].First(),
            Description = "Transit To Streets (Under Bridge)",
            ExfiltrationTime = 5f,
            ActivateAfterSeconds = 0,
            IsActive = true,
            Events = false,
            HideIfNoKey = false,
            X = -467.818f,
            Y = 8.167f,
            Z = -524.334f,
            RotationY = 331.695f,
            ConnectedIdentifier = "VGB_ST_WD"
        },
        // Railway Brige To Tarkov
        // new CustomExfil
        // {
        //     Identifier = "unique_identifier",
        //     IsTransit = true,
        //     TransitPointId = 0,// gets auto generated
        //     DestinationLocation = VagabondLocations.InverseLookupTable[RaidLocation.DESTINATION].First(),
        //     TargetLocation = VagabondLocations.InverseLookupTable[RaidLocation.DESTINATION].First(),
        //     Description = "Description",
        //     ExfiltrationTime = 5f,
        //     ActivateAfterSeconds = 0,
        //     IsActive = true,
        //     Events = false,
        //     HideIfNoKey = false,
        //     X = -728.767f,
        //     Y = 7.611f,
        //     Z = 130.17f,
        //     RotationY = 82.471f
        //         ConnectedIdentifier = "VGB_"
        // },
    ];
}