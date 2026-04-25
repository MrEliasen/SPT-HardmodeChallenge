using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Utils.Json;
using Vagabond.Server.Config;

namespace Vagabond.Server.Data.Quests;

public static class AddTherapistToHideoutQuest
{
    public const string QuestId = "69ebf5f00000000000000000";
    public const string TraderId = "54cb57776803fa99248b456e";
    private const string HandoverConditionMedsId = "69ebf62c0000000000000000";
    private const string HandoverConditionPkId = "69ebf6680000000000000000";
    private const string HandoverConditionHemoId = "69ebf6a40000000000000000";
    private const string HandoverConditionSplintId = "69ebf6e00000000000000000";
    private const string HandoverConditionSurgicalId = "69ebf71c0000000000000000";
    private const string RepConditionId = "69ebf7580000000000000000";

    public static NewQuestDetails Config()
    {
        return new NewQuestDetails()
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
            QuestName = "Stocking the Clinic",
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

            TraderId = TraderId,
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
                        Id = RepConditionId,
                        ConditionType = "TraderLoyalty",
                        DynamicLocale = false,
                        TraderId = TraderId,
                        CompareMethod = ">=",
                        Value = VagabondConfig.Config.JoinHideoutTherapistLoyaltyLevel,
                    },
                    new()
                    {
                        Id = HandoverConditionMedsId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        Value = 15,
                        OnlyFoundInRaid = true,
                        Target = new ListOrT<string>(new List<string>
                        {
                            "590c657e86f77412b013051d", // grizzly
                            "590c661e86f7741e566b646a", // Car kit
                            "590c678286f77426c9660122", // IFAK
                            "544fb45d4bdc2dee738b4568", // Salewa
                            "60098ad7c2240c0fe85c570a", // AFAK
                            "5755356824597772cb798962", // AI-2 medkit
                        }, null),
                    },
                    new()
                    {
                        Id = HandoverConditionPkId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        Value = 10,
                        OnlyFoundInRaid = true,
                        Target = new ListOrT<string>(new List<string>
                        {
                            "5af0548586f7743a532b7e99", // Ibuprofen 
                            "5751a89d24597722aa0e8db0", // Golden Star
                            "544fb37f4bdc2dee738b4567", // Analgin
                            "5755383e24597772cb798966", // Vaseline 
                            "60098ad7c2240c0fe85c570a", // Salewa
                        }, null),
                    },
                    new()
                    {
                        Id = HandoverConditionHemoId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        Value = 10,
                        OnlyFoundInRaid = true,
                        Target = new ListOrT<string>(new List<string>
                        {
                            "5e8488fa988a8701445df1e4", // CALOK-B 
                            "60098af40accd37ef2175f27", // CAT hemostatic
                            "5e831507ea0a7c419c2f9bd9", // Esmarch
                        }, null),
                    },
                    new()
                    {
                        Id = HandoverConditionSplintId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        Value = 10,
                        OnlyFoundInRaid = true,
                        Target = new ListOrT<string>(new List<string>
                        {
                            "544fb3364bdc2d34748b456a", // Immobilizing splint
                            "5af0454c86f7746bf20992e8", // Aluminum splint
                            "6937ecf8628ee476240c07cb", // Tigzresq - from WTT backport
                        }, null),
                    },
                    new()
                    {
                        Id = HandoverConditionSurgicalId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        Value = 5,
                        OnlyFoundInRaid = true,
                        Target = new ListOrT<string>(new List<string>
                        {
                            "5d02778e86f774203e7dedbe", // CMS surgical kit
                            "5d02797c86f774203f38e30a", // Surv12 field surgical kit
                        }, null),
                    },
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
        var en = new Dictionary<string, string>
        {
            [$"{QuestId} name"] = "Stocking the Clinic",
            [$"{QuestId} description"] =
                "Good day. I am preparing to leave my clinic, but I cannot do so under current conditions. There are still wounded, and supplies are... insufficient.\n\n" +
                "Before I can relocate, I need to ensure that what remains is properly stocked. Medicine, stimulants, basic supplies, anything that can help those who might still come through those doors.\n\n" +
                "Bring me what you can. Quantity matters here. Lives may depend on it.\n\n" +
                "** Completing this quest allows Therapist to be accessible from your hideout **",
            [$"{QuestId} startedMessageText"] =
                "I do not ask this lightly. My clinic cannot be abandoned while it is still in need. Bring me medical supplies, enough to leave it in a stable condition. Only then can I consider moving.",
            [$"{QuestId} successMessageText"] =
                "This will do. The clinic is properly stocked now... it should sustain those who remain for some time. Thank you. I can finally make preparations to leave. You have done something meaningful here.",
            [$"{QuestId} failMessageText"] = "That is unfortunate. We cannot proceed like this.",
            [$"{QuestId} acceptPlayerMessage"] = "I understand. I’ll bring what you need.",
            [$"{QuestId} declinePlayerMessage"] = "I can’t help with that.",
            [$"{QuestId} completePlayerMessage"] = "Here are the supplies.",
            [HandoverConditionMedsId] = "Hand over meds",
            [HandoverConditionPkId] = "Hand over pain killers",
            [HandoverConditionHemoId] = "Hand over hemostatics",
            [HandoverConditionSplintId] = "Hand over splints",
            [HandoverConditionSurgicalId] = "Hand over surgicals",
        };

        return new Dictionary<string, Dictionary<string, string>>
        {
            ["en"] = en
        };
    }
}