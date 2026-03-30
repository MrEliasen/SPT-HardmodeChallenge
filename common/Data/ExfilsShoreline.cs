using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Data;

public class ExfilsShoreline : ICustomExtilData
{
    public string MapName => _mapName;
    public List<CustomExfil> Extracts => _extracts;
    public List<CustomExfil> Transits => _transits;
    
    private static RaidLocation _raid = RaidLocation.Shoreline;
    public RaidLocation Raid => _raid;
    
    private static string _mapName = "Shoreline";
    private static List<CustomExfil> _extracts = [];
    private static  List<CustomExfil>_transits = [
        new CustomExfil
        {
            Identifier = "VGB_SL_CU",
            IsTransit = true,
            TransitPointId = 0,// gets auto generated
            DestinationLocation = LocationData.InverseLookupTable[RaidLocation.Customs].First(),
            TargetLocation = LocationData.InverseLookupTable[RaidLocation.Customs].First(),
            Description = "Transit to Customs",
            ExfiltrationTime = 20f,
            ActivateAfterSeconds = 60,
            IsActive = true,
            Events = false,
            HideIfNoKey = false,
            X = -855.903f,
            Y = -42.52f,
            Z = 10.129f,
            RotationY = 7.715f
        },
        new CustomExfil
        {
            Identifier = "VGB_SL_RS",
            IsTransit = true,
            TransitPointId = 0,// gets auto generated
            DestinationLocation = LocationData.InverseLookupTable[RaidLocation.Reserve].First(),
            TargetLocation = LocationData.InverseLookupTable[RaidLocation.Reserve].First(),
            Description = "Transit to Reserve",
            ExfiltrationTime = 20f,
            ActivateAfterSeconds = 60,
            IsActive = true,
            Events = false,
            HideIfNoKey = false,
            X = -199.952f,
            Y = -12.753f,
            Z = -356.269f,
            RotationY = 23.539f,
            Requirements = new() // Only works for extractions for now.. need to make a custom patch .. later keep as reminder
            {
                new CustomExtractRequirementDefinition
                {
                    Type = CustomExfilRequirementType.HasItem,
                    Id = "5c012ffc0db834001d23f03f", // Red Rebel
                    Count = 1,
                    RequirementTip = "Requires Red Rebel"
                },
                new CustomExtractRequirementDefinition
                {
                    Type = CustomExfilRequirementType.HasItem,
                    Id = "5d80c60f86f77440373c4ece", // Paracord
                    Count = 1,
                    RequirementTip = "Requires Paracord"
                }
            }
        },
        new CustomExfil
        {
            Identifier = "VGB_SL_LH",
            IsTransit = true,
            TransitPointId = 0,// gets auto generated
            DestinationLocation = LocationData.InverseLookupTable[RaidLocation.Lighthouse].First(),
            TargetLocation = LocationData.InverseLookupTable[RaidLocation.Lighthouse].First(),
            Description = "Transit to Lighthouse",
            ExfiltrationTime = 20f,
            ActivateAfterSeconds = 60,
            IsActive = true,
            Events = false,
            HideIfNoKey = false,
            X = 364.809f,
            Y = -59.601f,
            Z = 333.119f,
            RotationY = 235.559f
        }
    ];
}