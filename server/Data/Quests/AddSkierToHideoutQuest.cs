using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Utils.Json;
using Vagabond.Common.Data;
using Vagabond.Server.Config;

namespace Vagabond.Server.Data.Quests;

public static class AddSkierToHideoutQuest
{
    public const string QuestId = "69ecacc00000000000000000";
    public const string TraderId = "58330581ace78e27b8b10cee";
    private const string HandoverConditionCashId = "69ecacfc0000000000000000";
    private const string HandoverConditionValuablesId = "69ecad380000000000000000";
    private const string HandoverConditionIntelId = "69ecad740000000000000000";
    private const string RepConditionId = "69ecadb00000000000000000";

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
            QuestName = "New Collection",
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
                AvailableForStart = new List<QuestCondition>
                {
                    new()
                    {
                        Id = RepConditionId,
                        ConditionType = "TraderLoyalty",
                        DynamicLocale = false,
                        TraderId = TraderId,
                        CompareMethod = ">=",
                        Value = VagabondConfig.Config.JoinHideoutSkierLoyaltyLevel,
                    },
                },
                AvailableForFinish = new List<QuestCondition>
                {
                    new()
                    {
                        Id = RepConditionId,
                        ConditionType = "TraderLoyalty",
                        DynamicLocale = false,
                        TraderId = TraderId,
                        CompareMethod = ">=",
                        Value = VagabondConfig.Config.JoinHideoutSkierLoyaltyLevel,
                    },
                    new()
                    {
                        Id = HandoverConditionCashId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        Value = 8000,
                        OnlyFoundInRaid = false,
                        Target = new ListOrT<string>(new List<string>
                        {
                            Currencies.Euro,
                        }, null),
                    },
                    new()
                    {
                        Id = HandoverConditionValuablesId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        GlobalQuestCounterId = "",
                        Target = new ListOrT<string>([
                            "59faf7ca86f7740dbe19f6c2", // Roler
                            "5d235a5986f77443f6329bc6", // Gold skull ring
                            "62a09cfe4f842e1bd12da3e4", // Golden egg
                            "5734758f24597738025ee253", // Golden neck chain
                            "5bc9bc53d4351e00367fbcee", // Golden rooster figurine
                            "590de71386f774347051a052", // Antique teapot
                            "590de7e986f7741b096e5f32", // Antique vase
                            "5bc9c049d4351e44f824d360", // Battered antique book
                            "59e3639286f7741777737013", // Bronze lion figurine
                            "59e3658a86f7741776641ac4", // Cat figurine
                            "59e3647686f774176a362507", // Wooden clock
                        ], null),
                        Value = 8,
                        DogtagLevel = 0,
                        IsEncoded = false,
                        OnlyFoundInRaid = true,
                        Index = 0,
                        ParentId = "",
                        VisibilityConditions = new List<VisibilityCondition>()
                    },
                    new()
                    {
                        Id = HandoverConditionIntelId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        GlobalQuestCounterId = "",
                        Target = new ListOrT<string>([
                            "590c645c86f77412b01304d9", // Diary
                            "5c12613b86f7743bbe2c3f76", // Intelligence folder
                            "62a0a16d0b9d3c46de5b6e97", // Military flash drive
                            "590c621186f774138d11ea29", // Secure Flash drive
                            "61bf7c024770ee6f9c6b8b53", // Secure magnetic tape cassette
                            "590c651286f7741e566b6461", // Slim diary
                            "6389c8c5dbfd5e4b95197e6b", // TerraGroup "Blue Folders" materials
                        ], null),
                        Value = 3,
                        DogtagLevel = 0,
                        IsEncoded = false,
                        OnlyFoundInRaid = true,
                        Index = 0,
                        ParentId = "",
                        VisibilityConditions = new List<VisibilityCondition>()
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
            [$"{QuestId} description"] =
                "What, back again? Heh.. starting to think you like working for me.\n\n" +
                "Listen up. My usual spots are burned, too many leaks, too many idiots talking when they shouldn't. Business doesn't stop though, so I need a new place to run things from.\n\n" +
                "Your hideout? Quiet, tucked away. No traffic, no questions. That's exactly what I need.\n\n" +
                "But I'm not setting up shop in an empty room. I need real capital, euros, valuables, stuff that actually moves. And a bit of intel wouldn't hurt either.. information sells just as well as anything else these days.\n\n" +
                "You bring me something worth dealing with, and I'll start running operations out of your place. Keep it clean, keep it quiet, and we both make money.\n\n" +
                "** Completing this quest allows Skier to be accessible from your hideout **",
            [$"{QuestId} startedMessageText"] =
                "Don't waste my time. Euros, valuables, intel. If I can't flip it, I don't want it.",
            [$"{QuestId} successMessageText"] =
                "Now we're talking. This is clean, valuable.. exactly what I need.\n\n" +
                "Alright, I'll move things into your hideout. Small operation, nothing flashy. The kind of place people don't ask about.\n\n" +
                "You did good. Don’t get comfortable though, I don’t keep people around unless they stay useful.",
            [$"{QuestId} failMessageText"] =
                "What the hell is this? You expect me to run deals with this garbage?",
            [$"{QuestId} acceptPlayerMessage"] =
                "Fine. I’ll get your stuff.",
            [$"{QuestId} declinePlayerMessage"] =
                "Not my problem.",
            [$"{QuestId} completePlayerMessage"] =
                "Here. Euros, valuables, and intel.",
            [$"{QuestId} changeQuestMessageText"] =
                "Still not enough to get things moving. Bring me something worth my time.",
            [HandoverConditionCashId] = "Hand over euros",
            [HandoverConditionValuablesId] = "Hand over valuables",
            [HandoverConditionIntelId] = "Hand over intel items",
        };

        return new Dictionary<string, Dictionary<string, string>>
        {
            ["en"] = en
        };
    }
}