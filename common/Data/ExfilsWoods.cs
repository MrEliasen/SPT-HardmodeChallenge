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

    private static List<CustomExfil> _extracts =
    [
        new CustomExfil
        {
            Identifier = "VGB_EXT_JAEGER",
            DisplayName = "Jaegers Hideout",
            IsTransit = false,
            TemplateExitName = "", // only fill if you want a specific template
            EntryPoints = "",
            ExfiltrationTime = 20f,
            X = -155.956f,
            Y = 51.164f,
            Z = -273.296f,
            RotationY = 169.732f,
            Side = "Pmc"
        },
        // new CustomExfil
        // {
        //     Identifier = "VGB_EXT_ZB014",
        //     DisplayName = "Hideout Entrance",
        //     IsTransit = false,
        //     TemplateExitName = "",// only fill if you want a specific template
        //     EntryPoints = "",
        //     ExfiltrationTime = 20f,
        //     X = -155.941f,
        //     Y = 51.192f,
        //     Z = -273.616f,
        //     RotationY = 149.697f,
        //     Side = "Pmc"
        // },
        // new CustomExfil
        // {
        //     Identifier = "VGB_EXT_ZB016",
        //     DisplayName = "Hideout Entrance",
        //     IsTransit = false,
        //     TemplateExitName = "",// only fill if you want a specific template
        //     EntryPoints = "",
        //     ExfiltrationTime = 20f,
        //     X = -388.5f,
        //     Y = 3.327f,
        //     Z = 13.784f,
        //     RotationY = 7.122f,
        //     Side = "Pmc"
        // },
    ];

    private static List<CustomExfil> _transits =
    [
        new CustomExfil
        {
            Identifier = "VGB_WD_ST",
            IsTransit = true,
            TransitPointId = 0, // gets auto generated
            DestinationLocation = VagabondLocations.InverseLookupTable[RaidLocation.Streets].First(),
            TargetLocation = VagabondLocations.InverseLookupTable[RaidLocation.Streets].First(),
            Description = "Transit To Streets (Under Bridge)",
            ExfiltrationTime = 15f,
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
    ];
}