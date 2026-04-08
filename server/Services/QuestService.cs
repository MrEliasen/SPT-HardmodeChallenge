using Vagabond.Common.Data;
using Vagabond.Server.State;

namespace Vagabond.Server.Services;

public static class QuestService
{
    public static Dictionary<string, List<string>> BuildExfilList(VagabondState state)
    {
        Dictionary<string, List<string>> exfilList = new();

        if (state.QuestExfils.Count == 0)
        {
            return exfilList;
        }

        foreach (var quests in state.QuestExfils)
        {
            ExfilQuests.List.TryGetValue(quests, out var list);
            if (list == null)
            {
                continue;
            }

            foreach (var quest in list)
            {
                if (!exfilList.TryGetValue(quest.Key, out var exfils))
                {
                    exfils = new List<string>();
                    exfilList.Add(quest.Key, exfils);
                }

                foreach (var exfil in quest.Value)
                {
                    if (!exfils.Contains(exfil))
                    {
                        exfils.Add(exfil);
                    }
                }
            }
        }

        return exfilList;
    }
}