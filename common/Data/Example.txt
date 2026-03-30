using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;
using Vagabond.Common.Models;

namespace Vagabond.Common.Data;

public class ExfilsShoreline : ICustomExtilData
{
    public string MapName => _mapName;
    public List<CustomExfilDefinition> Extracts => _extracts;
    public List<CustomExfilDefinition> Transits => _transits;
    
    private static RaidLocation _raid = RaidLocation.Shoreline;
    public RaidLocation Raid => _raid;
    
    private static string _mapName = "shoreline";
    private static List<CustomExfilDefinition> _extracts = [
        new CustomExfilDefinition{
            Identifier = "vagabond_shoreline_cliff_vex",
            DisplayName = "Vagabond Cliff V-Ex",
            IsTransit = false,
            TemplateExitName = "Road to Customs",
            EntryPoints = "",
            ExfiltrationTime = 20f,
            X = -848.76f,
            Y = -42.364f,
            Z = 2.421f,
            RotationY = 30f,
            Side = "Pmc"
        }
    ];
    private static  List<CustomExfilDefinition>_transits = [
        new CustomExfilDefinition{
            Identifier = "vagabond_shoreline_to_customs",
            DisplayName = "Vagabond Transit to Customs",
            IsTransit = true,
            TransitPointId = 9001,
            DestinationLocation =  LocationData.InverseLookupTable[RaidLocation.Customs].First(),
            TargetLocation =  LocationData.InverseLookupTable[RaidLocation.Customs].First(),
            Description = "Move from Shoreline to Customs",
            ExfiltrationTime = 20f,
            ActivateAfterSeconds = 60,
            IsActive = true,
            Events = false,
            HideIfNoKey = false,
            X = -861.50f,
            Y = -42.364f,
            Z = 8.250f,
            RotationY = 65f
        }
    ];
}