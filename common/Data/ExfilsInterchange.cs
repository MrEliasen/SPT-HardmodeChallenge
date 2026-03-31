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
    private static List<CustomExfil> _extracts = [];
    private static  List<CustomExfil>_transits = [
        // // top-right 
        // new CustomExfil
        // {
        //     Identifier = "unique_identifier",
        //     DisplayName = "Label",
        //     IsTransit = true,
        //     TransitPointId = 0,// gets auto generated
        //     DestinationLocation = VagabondLocations.InverseLookupTable[RaidLocation.DESTINATION].First(),
        //     TargetLocation = VagabondLocations.InverseLookupTable[RaidLocation.DESTINATION].First(),
        //     Description = "Description",
        //     ExfiltrationTime = 20f,
        //     ActivateAfterSeconds = 60,
        //     IsActive = true,
        //     Events = false,
        //     HideIfNoKey = false,
        //     X = -317.718f,
        //     Y = 21.339f,
        //     Z = 268.838f,
        //     RotationY = 202.714f
        // },
        // // bottom right
        // new CustomExfil
        // {
        //     Identifier = "unique_identifier",
        //     DisplayName = "Label",
        //     IsTransit = true,
        //     TransitPointId = 0,// gets auto generated
        //     DestinationLocation = VagabondLocations.InverseLookupTable[RaidLocation.DESTINATION].First(),
        //     TargetLocation = VagabondLocations.InverseLookupTable[RaidLocation.DESTINATION].First(),
        //     Description = "Description",
        //     ExfiltrationTime = 20f,
        //     ActivateAfterSeconds = 60,
        //     IsActive = true,
        //     Events = false,
        //     HideIfNoKey = false,
        //     X = -252.624f,
        //     Y = 21.32f,
        //     Z = -367.248f,
        //     RotationY = 96.547f
        // },
    ];
}