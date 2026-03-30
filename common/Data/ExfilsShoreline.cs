using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Data;

public class ExfilsShoreline : ICustomExtilData
{
    public string MapName => _mapName;
    public List<CustomExfilDefinition> Extracts => _extracts;
    public List<CustomExfilDefinition> Transits => _transits;
    
    private static RaidLocation _raid = RaidLocation.Shoreline;
    public RaidLocation Raid => _raid;
    
    private static string _mapName = "shoreline";
    private static List<CustomExfilDefinition> _extracts = [];
    private static  List<CustomExfilDefinition>_transits = [
        new CustomExfilDefinition
        {
            Identifier = "shoreline_to_customs",
            DisplayName = "Transit To Customs",
            IsTransit = true,
            TransitPointId = 0,// gets auto generated
            DestinationLocation = LocationData.InverseLookupTable[RaidLocation.Customs].First(),
            TargetLocation = LocationData.InverseLookupTable[RaidLocation.Customs].First(),
            Description = "",
            ExfiltrationTime = 20f,
            ActivateAfterSeconds = 60,
            IsActive = false,
            Events = false,
            HideIfNoKey = false,
            X = -855.903f,
            Y = -42.52f,
            Z = 10.129f,
            RotationY = 7.715f
        },
        new CustomExfilDefinition
        {
            Identifier = "shoreline_to_reserver",
            DisplayName = "Transit To Reserve",
            IsTransit = true,
            TransitPointId = 0,// gets auto generated
            DestinationLocation = LocationData.InverseLookupTable[RaidLocation.Reserve].First(),
            TargetLocation = LocationData.InverseLookupTable[RaidLocation.Reserve].First(),
            Description = "",
            ExfiltrationTime = 20f,
            ActivateAfterSeconds = 60,
            IsActive = false,
            Events = false,
            HideIfNoKey = false,
            X = -199.952f,
            Y = -12.753f,
            Z = -356.269f,
            RotationY = 23.539f,
            RequirementsDescription = "Requires Red Rebel + Paracord",
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
        new CustomExfilDefinition
        {
            Identifier = "Shoreline_to_lighthouse",
            DisplayName = "Transit To Lighthouse",
            IsTransit = true,
            TransitPointId = 0,// gets auto generated
            DestinationLocation = LocationData.InverseLookupTable[RaidLocation.Lighthouse].First(),
            TargetLocation = LocationData.InverseLookupTable[RaidLocation.Lighthouse].First(),
            Description = "",
            ExfiltrationTime = 20f,
            ActivateAfterSeconds = 60,
            IsActive = false,
            Events = false,
            HideIfNoKey = false,
            X = 364.809f,
            Y = -59.601f,
            Z = 333.119f,
            RotationY = 235.559f
        }
    ];
}