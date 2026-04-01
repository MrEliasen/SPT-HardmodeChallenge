using Vagabond.Common.Data;
using Vagabond.Common.Enums;
using Vagabond.Server.Models;
using Vagabond.Server.Services;
using Vagabond.Server.State;

namespace Vagabond.Server.Data;

public static class StaticMapTransitions
{
    public  static ManualSpawnPoint? GetSpawnLocation(VagabondState state)
    {
        // if we cannot map the exit directly to a spawn location, we fall back to the below switch.
        if (GetTransitSpecificSpawnLocation(state.TransitState, out var customTransitSpawn))
        {
            return customTransitSpawn;
        }
        
        // if we cannot map the exit directly to a spawn location, we fall back to the below switch.
        if (GetNormalRaidLocation(state, out var customExitSpawn))
        {
            return customExitSpawn;
        }

        // Honestly, I made this as my first version of custom spawn points and it works well for
        // original transits, but if you have two transits going in the same direction, it falls apart.
        // So, leave this for original transits, and we only worry about mapping our custom transits.
        var from = VagabondLocations.NormaliseMapName(state.TransitState?.FromMap);
        var to = VagabondLocations.NormaliseMapName(state.TransitState?.ToMap);
        
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
                { X = 20.088f, Y = -0.365f, Z = -51.966f, Rotation = 327.838f },
            (RaidLocation.Woods, RaidLocation.FactoryDay) => new ManualSpawnPoint
                { X = 421.755f, Y = 1.168f, Z = 63.827f, Rotation = 182.888f },
            (RaidLocation.Labs, RaidLocation.FactoryDay) => new ManualSpawnPoint
                { X = -24.79f, Y = -3.562f, Z = -25.751f, Rotation = 55.127f },
            
            // Factory Night
            (RaidLocation.Customs, RaidLocation.FactoryNight) => new ManualSpawnPoint
                { X = 20.088f, Y = -0.365f, Z = -51.966f, Rotation = 327.838f },
            (RaidLocation.Woods, RaidLocation.FactoryNight) => new ManualSpawnPoint
                { X = 421.755f, Y = 1.168f, Z = 63.827f, Rotation = 182.888f },
            (RaidLocation.Labs, RaidLocation.FactoryNight) => new ManualSpawnPoint
                { X = -24.79f, Y = -3.562f, Z = -25.751f, Rotation = 55.127f },

            
            // Labs
            _ => null
        };
    }
    
    private static bool GetNormalRaidLocation(VagabondState state, out ManualSpawnPoint customExitSpawn)
    {
        customExitSpawn = null!;
        var raid  = VagabondLocations.NormaliseMapName(state.CurrentMap);
        var exitName = state.LastExit;
        
        if (raid == RaidLocation.Nil || string.IsNullOrEmpty(exitName))
        {
            return false;
        }
        
        var raidMapData = ExfilService.GetCustomMapData(raid);
        var exfil = raidMapData.Extracts.Concat(raidMapData.Transits).FirstOrDefault(x=> x.Identifier == exitName);
        if (exfil == null)
        {
            return false;
        }
        
        customExitSpawn = new ManualSpawnPoint{ X = exfil.X, Y = exfil.Y, Z = exfil.Z, Rotation = exfil.RotationY};
        //VagabondLogger.Error($"forcing spawn at {customExitSpawn.X},{customExitSpawn.Y},{customExitSpawn.Z},R={customExitSpawn.Rotation}");
        return true;
    }

    private static bool GetTransitSpecificSpawnLocation(TransitState? transitState, out ManualSpawnPoint customTransitSpawn)
    {
        customTransitSpawn = null!;
        if (transitState == null)
        {
            return false;
        }
        
        var from = VagabondLocations.NormaliseMapName(transitState.FromMap);
        var to = VagabondLocations.NormaliseMapName(transitState.ToMap);
        if (to == RaidLocation.Nil)
        {
            VagabondLogger.Error($"Raid is Nil, no specific spawn locations found");
            return false;
        }
        
        var fromMapData = ExfilService.GetCustomMapData(from);
        var exfil = fromMapData.Extracts.Concat(fromMapData.Transits).FirstOrDefault(x=> x.Identifier ==  transitState.ExitName);
        if (exfil == null)
        {
            //VagabondLogger.Error($"No exfils for map {to} from exit {transitState.ExitName}");
            return false;
        }
        
        if (string.IsNullOrEmpty(exfil.ConnectedIdentifier))
        {
            VagabondLogger.Error($"No ConnectedIdentifier on map {to}, exit {exfil.Identifier}");
            return false;
        }
        
        var toMapData = ExfilService.GetCustomMapData(to);
        var position = toMapData.Extracts.Concat(toMapData.Transits).FirstOrDefault(x=> x.Identifier ==  exfil.ConnectedIdentifier);
        if (position == null)
        {
            VagabondLogger.Error($"No connected spawn found for {transitState.ExitName} on {to}");
            return false;
        }
        
        customTransitSpawn = new ManualSpawnPoint{ X = position.X, Y = position.Y, Z = position.Z, Rotation = position.RotationY};
        return true;
    }
}