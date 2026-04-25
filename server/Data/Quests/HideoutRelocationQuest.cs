using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Utils.Json;
using Vagabond.Common.Data;
using Vagabond.Server.Config;

namespace Vagabond.Server.Data.Quests;

public static class HideoutRelocationQuest
{
    public const string QuestId = "69eaa4700000000000000000";
    private const string SkierId = "58330581ace78e27b8b10cee";
    private const string HandoverConditionId = "69eab2800000000000000000";

    public static NewQuestDetails Config()
    {
        return new NewQuestDetails
        {
            NewQuest = QuestConfig(),
            Locales = QuestLocales(),
            LockedToSide = null
        };
    }

    private static Quest QuestConfig()
    {
        return new Quest
        {
            Id = QuestId,
            QuestName = "Clean Slate",
            Name = $"{QuestId} name",
            Description = $"{QuestId} description",
            Note = $"{QuestId} note",
            StartedMessageText = $"{QuestId} startedMessageText",
            SuccessMessageText = $"{QuestId} successMessageText",
            FailMessageText = $"{QuestId} failMessageText",
            AcceptPlayerMessage = $"{QuestId} acceptPlayerMessage",
            DeclinePlayerMessage = $"{QuestId} declinePlayerMessage",
            CompletePlayerMessage = $"{QuestId} completePlayerMessage",
            ChangeQuestMessageText = $"{QuestId} changeQuestMessageText",

            TraderId = SkierId,
            Location = "any",
            Image = "/files/quest/icon/default.png",
            Type = QuestTypeEnum.Completion,
            Side = "Pmc",
            Restartable = true,
            CanShowNotificationsInGame = true,
            InstantComplete = false,
            SecretQuest = false,
            IsKey = false,
            TemplateId = QuestId,

            Conditions = new QuestConditionTypes
            {
                AvailableForStart = new List<QuestCondition>(),
                AvailableForFinish = new List<QuestCondition>
                {
                    new()
                    {
                        Id = HandoverConditionId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        GlobalQuestCounterId = "",
                        Value = VagabondConfig.Config.HideoutRelocationFee,
                        Target = new ListOrT<string>(new List<string>
                        {
                            Currencies.Ruble
                        }, null),
                        DogtagLevel = 0,
                        IsEncoded = false,
                        MaxDurability = 100,
                        MinDurability = 0,
                        Index = 0,
                        ParentId = "",
                        OnlyFoundInRaid = false,
                        VisibilityConditions = new List<VisibilityCondition>()
                    }
                },
                Fail = new List<QuestCondition>()
            },

            Rewards = new Dictionary<string, List<Reward>>
            {
                ["Started"] = new(),
                ["Fail"] = new(),
                ["Success"] = new(),
            }
        };
    }

    private static Dictionary<string, Dictionary<string, string>> QuestLocales()
    {
        var id = QuestId;

        var en = new Dictionary<string, string>
        {
            [$"{id} name"] = "Clean Slate",
            [$"{id} description"] =
                "So.. your little hideout setup. You really thought no one would notice? Come on. Everything gets found eventually. Question is, who finds it first.\n\n" +
                "Lucky for you, I know people who specialize in.. making problems disappear. Move the whole thing, clean slate, no loose ends. Quiet work. Expensive work.\n\n" +
                "You want a relocation, you pay. Simple as that. No favors, no credit.\n\n" +
                "Bring the roubles, I make the call, and your exit gets.. relocated. After that? Try not to screw it up again.\n\n" +
                "** Completing this quests allows you to relocate your hideout (Repeatable) **",
            [$"{id} startedMessageText"] =
                "Don't stall. These guys don't sit around waiting while you count pennies. Get the money, bring it in, and we'll move your little operation.",
            [$"{id} successMessageText"] =
                "Good. Money's clean.\n\n" +
                "I've already made the call. My people will handle the relocation, fast, quiet, no questions asked.\n\n" +
                "You'll get your chance to set things up again. Just don't pick some obvious dump this time, yeah? I'm not doing this twice for free.",
            [$"{id} failMessageText"] =
                "Yeah, didn't think so. Come back when you're serious.",
            [$"{id} acceptPlayerMessage"] =
                "Fine. I'll get your money.",
            [$"{id} declinePlayerMessage"] =
                "Not worth it right now.",
            [$"{id} completePlayerMessage"] =
                "Here. Count it if you want.",
            [$"{id} changeQuestMessageText"] =
                "Still waiting on the cash. No money, no move.",
            [HandoverConditionId] = "Hand over Roubles",
        };

        return new Dictionary<string, Dictionary<string, string>>
        {
            ["en"] = en
        };
    }
}