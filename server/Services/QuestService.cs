using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Services.Mod;
using Vagabond.Common.Data;
using Vagabond.Common.Definitions;
using Vagabond.Server.Data.Quests;

namespace Vagabond.Server.Services;

public static class QuestService
{
    public static Dictionary<string, List<string>> BuildExfilList(VagabondSessionState state)
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

    public static void LoadQuests()
    {
        List<NewQuestDetails> quests =
        [
            HideoutRelocationQuest.Config(),
            AddTherapistToHideoutQuest.Config(),
            AddJaegerToHideoutQuest.Config(),
            AddMechanicToHideoutQuest.Config(),
            AddPeacekeeperToHideoutQuest.Config(),
            AddPraporToHideoutQuest.Config(),
            AddRagmanToHideoutQuest.Config(),
            AddSkierToHideoutQuest.Config(),
        ];

        var customQuestService = ReflectionUtil.GetService<CustomQuestService>();
        if (customQuestService == null)
        {
            VagabondLogger.Warning($"failed to load quests, customQuestService is null.");
            return;
        }

        var databaseService = ReflectionUtil.GetService<DatabaseService>();
        var supportedLocales = databaseService?.GetLocales().Global.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase)
                               ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "en" };

        foreach (var quest in quests)
        {
            var defaultLocale = quest.Locales.TryGetValue("en", out var en)
                ? en
                : quest.Locales.First().Value;

            foreach (var lang in supportedLocales)
            {
                quest.Locales.TryAdd(lang, defaultLocale);
            }

            var result = customQuestService.CreateQuest(quest);
            if (!result.Success)
            {
                foreach (var err in result.Errors)
                {
                    VagabondLogger.Warning($"quest registration of quest id {quest.NewQuest.Id}, error: {err}");
                }
            }
        }
    }
}