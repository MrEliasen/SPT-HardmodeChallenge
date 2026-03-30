using Vagabond.Common.Enums;

namespace Vagabond.Common.Definitions;

public class LocationData
{
    public static Dictionary<RaidLocation, List<string>> Locations = new()
    {
        [RaidLocation.Factory] = ["55f2d3fd4bdc2d5f408b4567", "59fc81d786f774390775787e"],
        [RaidLocation.GroundZero] =
            ["65b8d6f5cdde2479cb2a3125", "653e6760052c01c1c805532f", "68236e8153654e8c1200798a"],
        [RaidLocation.Streets] = ["5714dc692459777137212e12"],
        [RaidLocation.Woods] = ["5704e3c2d2720bac5b8b4567"],
        [RaidLocation.Customs] = ["56f40101d2720b2a4d8b45d6"],
        [RaidLocation.Interchange] = ["5714dbc024597771384a510d"],
        [RaidLocation.Lighthouse] = ["5704e4dad2720bb55b8b4567"],
        [RaidLocation.Reserve] = ["5704e5fad2720bc05b8b4567"],
        [RaidLocation.Shoreline] = ["5704e554d2720bac5b8b456e"],
        [RaidLocation.Labs] = ["5b0fc42d86f7744a585f9105"],
        [RaidLocation.Labyrinth] = ["6733700029c367a3d40b02af"],
    };

    public static Dictionary<string, string> IdToName = new(StringComparer.OrdinalIgnoreCase)
    {
        //EFT
        ["56f40101d2720b2a4d8b45d6"] = "bigmap",
        ["55f2d3fd4bdc2d5f408b4567"] = "factory4_day",
        ["653e6760052c01c1c805532f"] = "sandbox",
        ["65b8d6f5cdde2479cb2a3125"] = "sandbox_high",
        ["68236e8153654e8c1200798a"] = "sandbox_start",
        ["5714dbc024597771384a510d"] = "interchange",
        ["5704e4dad2720bb55b8b4567"] = "lighthouse",
        ["59fc81d786f774390775787e"] = "factory4_night",
        ["5704e5fad2720bc05b8b4567"] = "rezervbase",
        ["5704e554d2720bac5b8b456e"] = "shoreline",
        ["5714dc692459777137212e12"] = "tarkovstreets",
        ["5b0fc42d86f7744a585f9105"] = "laboratory",
        ["6733700029c367a3d40b02af"] = "labyrinth",
        ["5704e3c2d2720bac5b8b4567"] = "woods",
    };

    public static Dictionary<RaidLocation, List<string>> InverseLookupTable = new()
    {
        [RaidLocation.Factory] = new List<string>
        {
            "factory4_day",
            "factory4_night",
            "Factory4Day",
            "Factory4Night",
        },
        [RaidLocation.GroundZero] = new List<string>
        {
            "sandbox_high",
            "sandbox",
            "sandbox_start",
            "SandboxHigh"
        },
        [RaidLocation.Streets] = new List<string>
        {
            "tarkovstreets",
        },
        [RaidLocation.Woods] = new List<string>
        {
            "woods",
        },
        [RaidLocation.Customs] = new List<string>
        {
            "bigmap",
        },
        [RaidLocation.Interchange] = new List<string>
        {
            "interchange",
        },
        [RaidLocation.Lighthouse] = new List<string>
        {
            "lighthouse",
        },
        [RaidLocation.Reserve] = new List<string>
        {
            "rezervbase",
        },
        [RaidLocation.Shoreline] = new List<string>
        {
            "shoreline",
        },
        [RaidLocation.Labs] = new List<string>
        {
            "laboratory",
        },
        [RaidLocation.Labyrinth] = new List<string>
        {
            "labyrinth",
        },
    };

    public static Dictionary<string, RaidLocation> LookupTable = new(StringComparer.OrdinalIgnoreCase)
    {
        //EFT
        ["factory4_day"] = RaidLocation.Factory,
        ["factory4_night"] = RaidLocation.Factory,
        ["sandbox"] = RaidLocation.GroundZero,
        ["sandbox_high"] = RaidLocation.GroundZero,
        ["sandbox_start"] = RaidLocation.GroundZero,
        ["tarkovstreets"] = RaidLocation.Streets,
        ["woods"] = RaidLocation.Woods,
        ["bigmap"] = RaidLocation.Customs,
        ["interchange"] = RaidLocation.Interchange,
        ["lighthouse"] = RaidLocation.Lighthouse,
        ["rezervbase"] = RaidLocation.Reserve,
        ["shoreline"] = RaidLocation.Shoreline,
        ["laboratory"] = RaidLocation.Labs,
        ["labyrinth"] = RaidLocation.Labyrinth,
        //SPT
        ["Factory4Day"] = RaidLocation.Factory,
        ["Factory4Night"] = RaidLocation.Factory,
        ["SandboxHigh"] = RaidLocation.GroundZero,
    };
    
    public static RaidLocation NormaliseMapName(string? mapName)
    {
        if (string.IsNullOrWhiteSpace(mapName))
        {
            return RaidLocation.Nil;
        }

        if (LookupTable.TryGetValue(mapName.ToLower(), out var mapped))
        {
            return mapped;
        }
        
        RaidLocation parsed = RaidLocation.Nil;
        RaidLocation.TryParse(mapName, true, out parsed);

        if (parsed != RaidLocation.Nil)
        {
            return parsed;
        }

        foreach (var loc in Locations)
        {
            if (loc.Value.Contains(mapName))
            {
                return loc.Key;
            }
        }
        
        return RaidLocation.Nil;
    }
}