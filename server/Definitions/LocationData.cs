using Vagabond.Server.Models.Enums;

namespace Vagabond.Server.Definitions;

public class LocationData
{
    public static Dictionary<RaidLocations, List<string>> Locations = new()
    {
        [RaidLocations.Factory] = ["55f2d3fd4bdc2d5f408b4567", "59fc81d786f774390775787e"],
        [RaidLocations.GroundZero] =
            ["65b8d6f5cdde2479cb2a3125", "653e6760052c01c1c805532f", "68236e8153654e8c1200798a"],
        [RaidLocations.Streets] = ["5714dc692459777137212e12"],
        [RaidLocations.Woods] = ["5704e3c2d2720bac5b8b4567"],
        [RaidLocations.Customs] = ["56f40101d2720b2a4d8b45d6"],
        [RaidLocations.Interchange] = ["5714dbc024597771384a510d"],
        [RaidLocations.Lighthouse] = ["5704e4dad2720bb55b8b4567"],
        [RaidLocations.Reserve] = ["5704e5fad2720bc05b8b4567"],
        [RaidLocations.Shoreline] = ["5704e554d2720bac5b8b456e"],
        [RaidLocations.Labs] = ["5b0fc42d86f7744a585f9105"],
        [RaidLocations.Labyrinth] = ["6733700029c367a3d40b02af"],
    };

    public static Dictionary<string, RaidLocations> LookupTable = new(StringComparer.OrdinalIgnoreCase)
    {
        //EFT
        ["factory4_day"] = RaidLocations.Factory,
        ["factory4_night"] = RaidLocations.Factory,
        ["sandbox"] = RaidLocations.GroundZero,
        ["sandbox_high"] = RaidLocations.GroundZero,
        ["sandbox_start"] = RaidLocations.GroundZero,
        ["tarkovstreets"] = RaidLocations.Streets,
        ["woods"] = RaidLocations.Woods,
        ["bigmap"] = RaidLocations.Customs,
        ["interchange"] = RaidLocations.Interchange,
        ["lighthouse"] = RaidLocations.Lighthouse,
        ["rezervbase"] = RaidLocations.Reserve,
        ["shoreline"] = RaidLocations.Shoreline,
        ["laboratory"] = RaidLocations.Labs,
        ["labyrinth"] = RaidLocations.Labyrinth,
        //SPT
        ["Factory4Day"] = RaidLocations.Factory,
        ["Factory4Night"] = RaidLocations.Factory,
        ["SandboxHigh"] = RaidLocations.GroundZero,
    };
}