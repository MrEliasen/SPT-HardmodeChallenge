namespace Vagabond.Common.Data;

using System.Collections.Generic;

public static class ExfilQuests
{
    public static readonly Dictionary<string, Dictionary<string, List<string>>> List = new()
    {
        // Back Door
        ["6089736efa70fc097863b8f6"] = new()
        {
            ["RezervBase"] = new() { "EXFIL_Bunker_D2" }
        },

        // Cease Fire
        ["639136e84ed9512be67647db"] = new()
        {
            ["TarkovStreets"] = new() { "E9_sniper" }
        },

        // Information Source
        ["63966faeea19ac7ed845db2c"] = new()
        {
            ["Woods"] = new() { "South V-Ex" },
            ["bigmap"] = new() { "Dorms V-Ex" },
            ["Interchange"] = new() { "PP Exfil" },
            ["TarkovStreets"] = new() { "E7_car" },
            ["Sandbox"] = new() { "Sandbox_VExit" },
            ["Sandbox_high"] = new() { "Sandbox_VExit" }
        },

        // Payback
        ["63966fd9ea19ac7ed845db30"] = new()
        {
            ["RezervBase"] = new() { "Alpinist" }
        },

        // Dangerous Road
        ["63ab180c87413d64ae0ac20a"] = new()
        {
            ["TarkovStreets"] = new() { "E7_car" }
        },

        // Ambulances Again
        ["64f3176921045e77405d63b5"] = new()
        {
            ["TarkovStreets"] = new() { "E7_car" }
        },

        // Burning Rubber
        ["657315e270bb0b8dba00cc48"] = new()
        {
            ["Sandbox"] = new() { "Sandbox_VExit" },
            ["Sandbox_high"] = new() { "Sandbox_VExit" }
        },

        // Exit Here
        ["669fa395c4c5c04798002497"] = new()
        {
            ["factory4_day"] = new() { "Gate_o" },
            ["factory4_night"] = new() { "Gate_o" }
        },

        // Belka and Strelka
        ["675c3507a06634b5110e3c18"] = new()
        {
            ["bigmap"] = new() { "customs_sniper_exit" }
        }
    };
}