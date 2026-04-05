using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;
using Vagabond.Common.Interfaces;
using Vagabond.Common.Models;

namespace Vagabond.Common.Data;

public class StaticTransitionSpawns
{
    public static ManualSpawnPoint? GetStaticSpawn(RaidLocation from, RaidLocation to)
    {
        return (from, to) switch
        {
            // Ground Zero
            (RaidLocation.Streets, RaidLocation.GroundZero) => new ManualSpawnPoint
                { X = 230.016f, Y = 16.187f, Z = 83.303f, Rotation = 231.771f },

            // Customs
            (RaidLocation.Interchange, RaidLocation.Customs) => new ManualSpawnPoint
                { X = -338.961f, Y = 0.793f, Z = -194.769f, Rotation = 30.629f },
            (RaidLocation.Shoreline, RaidLocation.Customs) => new ManualSpawnPoint
                { X = 24.013f, Y = -1.326f, Z = 134.716f, Rotation = 95.674f },
            (RaidLocation.Reserve, RaidLocation.Customs) => new ManualSpawnPoint
                { X = 650.311f, Y = 0.39f, Z = 116.193f, Rotation = 196.06f },
            (RaidLocation.FactoryDay, RaidLocation.Customs) => new ManualSpawnPoint
                { X = 353.939f, Y = 1.123f, Z = -189.197f, Rotation = 3.389f },
            (RaidLocation.FactoryNight, RaidLocation.Customs) => new ManualSpawnPoint
                { X = 353.939f, Y = 1.123f, Z = -189.197f, Rotation = 3.389f },
            (RaidLocation.Woods, RaidLocation.Customs) => new ManualSpawnPoint
                { X = -4.414f, Y = 1.104f, Z = -136.337f, Rotation = 352.916f },

            // Streets
            (RaidLocation.Labs, RaidLocation.Streets) => new ManualSpawnPoint
                { X = 210.119f, Y = -8.291f, Z = 82.166f, Rotation = 88.696f },
            (RaidLocation.GroundZero, RaidLocation.Streets) => new ManualSpawnPoint
                { X = -248.599f, Y = 2.245f, Z = 98.421f, Rotation = 19.081f },
            (RaidLocation.Interchange, RaidLocation.Streets) => new ManualSpawnPoint
                { X = 288.596f, Y = 3.469f, Z = 489.124f, Rotation = 227.398f },
            (RaidLocation.Woods, RaidLocation.Streets) => new ManualSpawnPoint
                { X = 288.596f, Y = 3.469f, Z = 489.124f, Rotation = 227.398f },

            // Interchange
            (RaidLocation.Streets, RaidLocation.Interchange) => new ManualSpawnPoint
                { X = 269.422f, Y = 21.401f, Z = -445.558f, Rotation = 339.283f },
            (RaidLocation.Customs, RaidLocation.Interchange) => new ManualSpawnPoint
                { X = 289.967f, Y = 21.341f, Z = 377.473f, Rotation = 274.522f },

            // Reserve
            (RaidLocation.Customs, RaidLocation.Reserve) => new ManualSpawnPoint
                { X = -200.731f, Y = -5.986f, Z = -107.305f, Rotation = 134.331f },
            (RaidLocation.Woods, RaidLocation.Reserve) => new ManualSpawnPoint
                { X = 35.423f, Y = -7.003f, Z = -221.638f, Rotation = 349.134f }, // exit to woods
            //(RaidLocation.Woods, RaidLocation.Reserve) => new ManualSpawnPoint { X = 216.246f, Y = -7.007f, Z = -176.805f, Rotation = 211.995f }, // woods exfil
            (RaidLocation.Lighthouse, RaidLocation.Reserve) => new ManualSpawnPoint
                { X = 216.246f, Y = -7.007f, Z = -176.805f, Rotation = 211.995f },

            // Woods
            (RaidLocation.FactoryDay, RaidLocation.Woods) => new ManualSpawnPoint
                { X = -355.201f, Y = -0.268f, Z = 362.391f, Rotation = 161.997f },
            (RaidLocation.FactoryNight, RaidLocation.Woods) => new ManualSpawnPoint
                { X = -355.201f, Y = -0.268f, Z = 362.391f, Rotation = 161.997f },
            (RaidLocation.Customs, RaidLocation.Woods) => new ManualSpawnPoint
                { X = -139.908f, Y = -1.504f, Z = 417.126f, Rotation = 212.588f },
            (RaidLocation.Reserve, RaidLocation.Woods) => new ManualSpawnPoint
                { X = 252.936f, Y = -9.516f, Z = 354.375f, Rotation = 135.734f },
            (RaidLocation.Lighthouse, RaidLocation.Woods) => new ManualSpawnPoint
                { X = 498.298f, Y = -17.483f, Z = 348.645f, Rotation = 231.116f },

            // Lighthouse
            (RaidLocation.Shoreline, RaidLocation.Lighthouse) => new ManualSpawnPoint
                { X = -343.76f, Y = 8.158f, Z = -160.048f, Rotation = 109.296f },
            (RaidLocation.Reserve, RaidLocation.Lighthouse) => new ManualSpawnPoint
                { X = -313.705f, Y = 15.432f, Z = -773.452f, Rotation = 122.249f },
            (RaidLocation.Woods, RaidLocation.Lighthouse) => new ManualSpawnPoint
                { X = 104.531f, Y = 4.642f, Z = -959.373f, Rotation = 5.686f },

            // Shoreline
            (RaidLocation.Customs, RaidLocation.Shoreline) => new ManualSpawnPoint
                { X = -848.76f, Y = -42.364f, Z = 2.421f, Rotation = 29.018f },
            (RaidLocation.Lighthouse, RaidLocation.Shoreline) => new ManualSpawnPoint
                { X = 418.876f, Y = -57.395f, Z = -191.697f, Rotation = 298.845f },

            // Factory Day
            (RaidLocation.Customs, RaidLocation.FactoryDay) => new ManualSpawnPoint
                { X = 19.901f, Y = -0.335f, Z = -49.05f, Rotation = 357.57f },
            (RaidLocation.Woods, RaidLocation.FactoryDay) => new ManualSpawnPoint
                { X = 22.337f, Y = 1.169f, Z = 63.884f, Rotation = 174.311f },
            (RaidLocation.Labs, RaidLocation.FactoryDay) => new ManualSpawnPoint
                { X = -24.79f, Y = -3.562f, Z = -25.751f, Rotation = 55.127f },

            // Factory Night
            (RaidLocation.Customs, RaidLocation.FactoryNight) => new ManualSpawnPoint
                { X = 19.901f, Y = -0.335f, Z = -49.05f, Rotation = 357.57f },
            (RaidLocation.Woods, RaidLocation.FactoryNight) => new ManualSpawnPoint
                { X = 22.337f, Y = 1.169f, Z = 63.884f, Rotation = 174.311f },
            (RaidLocation.Labs, RaidLocation.FactoryNight) => new ManualSpawnPoint
                { X = -24.79f, Y = -3.562f, Z = -25.751f, Rotation = 55.127f },

            // Labs
            // labyrinth
            // any other combination
            _ => null
        };
    }

    public static CustomExfil GetMapExtractTemplate(RaidLocation raid)
    {
        ICustomExtilData? exfilTemplate = (raid) switch
        {
            (RaidLocation.Customs) => new ExfilsCustoms(),
            (RaidLocation.FactoryDay) => new ExfilsFactoryDay(),
            (RaidLocation.FactoryNight) => new ExfilsFactoryNight(),
            (RaidLocation.GroundZero) => new ExfilsGroundZero(),
            (RaidLocation.Interchange) => new ExfilsInterchange(),
            (RaidLocation.Labs) => new ExfilsLabs(),
            (RaidLocation.Labyrinth) => new ExfilsLabyrinth(),
            (RaidLocation.Lighthouse) => new ExfilsLighthouse(),
            (RaidLocation.Reserve) => new ExfilsReserve(),
            (RaidLocation.Shoreline) => new ExfilsShoreline(),
            (RaidLocation.Streets) => new ExfilsStreets(),
            (RaidLocation.Woods) => new ExfilsWoods(),
            _ => null
        };

        if (exfilTemplate == null || exfilTemplate.Extracts.Count == 0)
        {
            return new CustomExfil
            {
                Identifier = "",
                DisplayName = "",
                IsTransit = false,
                TemplateExitName = "",
                EntryPoints = "",
                ExfiltrationTime = 0f,
                X = 0f,
                Y = 0f,
                Z = 0f,
                RotationY = 0f,
                Side = "Pmc"
            };
        }

        return exfilTemplate.Extracts.First();
    }
}