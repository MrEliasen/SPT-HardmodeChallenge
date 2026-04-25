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

    public static readonly Dictionary<string, List<string>> TraderQuests = new()
    {
        ["5a7c2eca46aef81a7ca2145d"] = ["6089736efa70fc097863b8f6"],
        ["5c0647fdd443bc2504c2d371"] = ["639136e84ed9512be67647db"],
        ["638f541a29ffd1183d187f57"] = ["63966faeea19ac7ed845db2c", "63966fd9ea19ac7ed845db30"],
        ["54cb57776803fa99248b456e"] = ["63ab180c87413d64ae0ac20a", "64f3176921045e77405d63b5"],
        ["58330581ace78e27b8b10cee"] = ["657315e270bb0b8dba00cc48", "669fa395c4c5c04798002497"],
        ["54cb50c76803fa8b248b4571"] = ["675c3507a06634b5110e3c18"],
    };

    public static bool IsExfilQuest(string exitName, HashSet<string> questExfils, out string traderId)
    {
        traderId = string.Empty;
        foreach (var quest in List)
        {
            if (!questExfils.Contains(quest.Key))
            {
                continue;
            }

            foreach (var exits in quest.Value)
            {
                if (exits.Value.Contains(exitName))
                {
                    traderId = TraderQuests.FirstOrDefault(x => x.Value.Contains(quest.Key)).Key;
                    return true;
                }
            }
        }

        return false;
    }
}