using HardmodeChallenge.Server.Models.Enums;

namespace HardmodeChallenge.Server.Definitions;

public class LocationData
{
    public static Dictionary<HCLocation, List<string>> Locations = new()
    {
        [HCLocation.Factory] = ["55f2d3fd4bdc2d5f408b4567", "59fc81d786f774390775787e"],
        [HCLocation.GroundZero] =
            ["65b8d6f5cdde2479cb2a3125", "653e6760052c01c1c805532f", "68236e8153654e8c1200798a"],
        [HCLocation.Streets] = ["5714dc692459777137212e12"],
        [HCLocation.Woods] = ["5704e3c2d2720bac5b8b4567"],
        [HCLocation.Customs] = ["56f40101d2720b2a4d8b45d6"],
        [HCLocation.Interchange] = ["5714dbc024597771384a510d"],
        [HCLocation.Lighthouse] = ["5704e4dad2720bb55b8b4567"],
        [HCLocation.Reserve] = ["5704e5fad2720bc05b8b4567"],
        [HCLocation.Shoreline] = ["5704e554d2720bac5b8b456e"],
        [HCLocation.Labs] = ["5b0fc42d86f7744a585f9105"],
        [HCLocation.Labyrinth] = ["6733700029c367a3d40b02af"],
    };

    public static Dictionary<string, HCLocation> LookupTable = new(StringComparer.OrdinalIgnoreCase)
    {
        //EFT
        ["factory4_day"] = HCLocation.Factory,
        ["factory4_night"] = HCLocation.Factory,
        ["sandbox"] = HCLocation.GroundZero,
        ["sandbox_high"] = HCLocation.GroundZero,
        ["sandbox_start"] = HCLocation.GroundZero,
        ["tarkovstreets"] = HCLocation.Streets,
        ["woods"] = HCLocation.Woods,
        ["bigmap"] = HCLocation.Customs,
        ["interchange"] = HCLocation.Interchange,
        ["lighthouse"] = HCLocation.Lighthouse,
        ["rezervbase"] = HCLocation.Reserve,
        ["shoreline"] = HCLocation.Shoreline,
        ["laboratory"] = HCLocation.Labs,
        ["labyrinth"] = HCLocation.Labyrinth,
        //SPT
        ["Factory4Day"] = HCLocation.Factory,
        ["Factory4Night"] = HCLocation.Factory,
        ["SandboxHigh"] = HCLocation.GroundZero,
    };
}