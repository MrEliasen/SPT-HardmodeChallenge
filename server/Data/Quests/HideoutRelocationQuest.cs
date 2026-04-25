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
            QuestName = "Fresh Foundations",
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
            [$"{id} name"] = "Fresh Foundations",
            [$"{id} description"] =
                "Here’s the deal. Your hideout? Not as hidden as you think. Places get found, raided, burned out. Happens all the time. But I know people who can move it. Quiet, fast, no traces.\n\nOf course, that kind of work ain’t cheap. You bring me the roubles, I make the call, and you get to set up somewhere new. Safer.\n\nUp to you; pay now, or wait until someone else finds it first.\n\n ** Completing this quests allows you to relocate your hideout (Repeatable) **",
            [$"{id} note"] = "",
            [$"{id} startedMessageText"] =
                "Smart choice. Get the money together and don’t drag your feet, these people don’t wait around for anyone. Bring it in, and we’ll get things moving",
            [$"{id} successMessageText"] =
                "That’s what I like to see. Money’s in, arrangements made. My people are ready and waiting on your word. Go find yourself a better spot and try not to pick somewhere obvious, yeah?",
            [$"{id} failMessageText"] = "Your loss.",
            [$"{id} acceptPlayerMessage"] = "Alright. I'll get your money.",
            [$"{id} declinePlayerMessage"] = "Not worth it.",
            [$"{id} completePlayerMessage"] = "It's all there.",
            [$"{id} changeQuestMessageText"] = "",
            [HandoverConditionId] = "Hand over Roubles",
        };

        return new Dictionary<string, Dictionary<string, string>>
        {
            ["en"] = en
        };
    }
}