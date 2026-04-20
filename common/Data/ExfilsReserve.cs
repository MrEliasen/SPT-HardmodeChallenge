using Vagabond.Common.Definitions;
using Vagabond.Common.Interfaces;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Data;

public class ExfilsReserve : ICustomExtilData
{
    public string MapName => _mapName;
    public List<CustomExfil> Extracts => _extracts;
    public List<CustomExfil> Transits => _transits;

    private static RaidLocation _raid = RaidLocation.Reserve;
    public RaidLocation Raid => _raid;

    private static string _mapName = "RezervBase";

    private static List<CustomExfil> _extracts =
    [
        new CustomExfil
        {
            Identifier = "VGB_EXT_MARKET",
            DisplayName = "Underground Market via Hermetic Door (Custom Traders)",
            TemplateExitName = "EXFIL_Bunker",
            HijackExfil = true, // it will take most information from the hijacked exfil
            // still needed to force spawning back into the map
            X = 62.5f,
            Y = -6.936f,
            Z = -193.896f,
            RotationY = 17.317f,
        },
    ];

    private static List<CustomExfil> _transits =
    [
        new CustomExfil
        {
            Identifier = "VGB_RS_SL",
            IsTransit = true,
            TransitPointId = 0, // gets auto generated
            DestinationLocation = VagabondLocations.InverseLookupTable[RaidLocation.Shoreline].First(),
            TargetLocation = VagabondLocations.InverseLookupTable[RaidLocation.Shoreline].First(),
            Description = "Mountain climb to Shoreline",
            ExfiltrationTime = 15f,
            ActivateAfterSeconds = 0,
            IsActive = true,
            Events = false,
            HideIfNoKey = false,
            X = -14.466f,
            Y = 18.409f,
            Z = 205.046f,
            RotationY = 179.555f,
            Requirements = new()
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
            },
            ConnectedIdentifier = "VGB_SL_RS",
        },
    ];
}