using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Utils.Json;
using Vagabond.Server.Config;

namespace Vagabond.Server.Data.Quests;

public static class AddJaegerToHideoutQuest
{
    public const string QuestId = "69ec82900000000000000000";
    public const string TraderId = "5c0647fdd443bc2504c2d371";
    private const string HandoverConditionFoodId = "69ec82cc0000000000000000";
    private const string HandoverConditionDrinksId = "69ec83080000000000000000";
    private const string RepConditionId = "69ec83800000000000000000";

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
            QuestName = "Prepper",
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
                        Target = new ListOrT<string>(null, TraderId),
                        CompareMethod = ">=",
                        Value = VagabondConfig.Config.JoinHideoutJaegerLoyaltyLevel,
                    },
                },
                AvailableForFinish = new List<QuestCondition>
                {
                    new()
                    {
                        Id = RepConditionId,
                        ConditionType = "TraderLoyalty",
                        DynamicLocale = false,
                        Target = new ListOrT<string>(null, TraderId),
                        CompareMethod = ">=",
                        Value = VagabondConfig.Config.JoinHideoutJaegerLoyaltyLevel,
                    },
                    new()
                    {
                        Id = HandoverConditionFoodId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        Value = 25,
                        OnlyFoundInRaid = true,
                        Target = new ListOrT<string>(new List<string>
                        {
                            "57505f6224597709a92585a9", // Alyonka chocolate bar
                            "5448ff904bdc2d6f028b456e", // Army crackers
                            "57347da92459774491567cf5", // Can of beef stew
                            "57347d7224597744596b4e72", // Can of beef stew (Small)
                            "57347d692459774491567cf1", // Can of green peas
                            "57347d9c245977448b40fa85", // Can of herring
                            "57347d5f245977448b40fa81", // Can of humpback salmon
                            "5673de654bdc2d180f8b456d", // Can of pacific saury
                            "5bc9c29cd4351e003562b8a3", // Can of sprats
                            "57347d8724597744596b4e76", // Can of squash spread
                            "590c5d4b86f774784e1b9c45", // Iskra ration pack
                            "590c5f0d86f77413997acfab", // MRE ration pack
                            "65815f0e647e3d7246384e14", // Pack of Tarker dried meat
                            "635a758bfefc88a93f021b8a", // Salty Dog beef sausage
                            "544fb6cc4bdc2d34748b456e", // Slickers chocolate bar
                            "59e3577886f774176a362503", // Pack of sugar
                        }, null),
                    },
                    new()
                    {
                        Id = HandoverConditionDrinksId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        Value = 25,
                        OnlyFoundInRaid = true,
                        Target = new ListOrT<string>(new List<string>
                        {
                            "5c0fa877d174af02a012e1cf", // Aquamari water bottle with filter
                            "5e8f3423fd7471236e6e3b64", // Bottle of Norvinsky Yadreniy premium kvass (0.6L)
                            "5448fee04bdc2dbc018b4567", // bottle of water (0.6L)
                            "5751496424597720a27126da", // Can of Hot Rod energy drink
                            "575062b524597720a31c09a1", // Can of Ice Green tea
                            "5751435d24597720a27126d1", // Can of Max Energy energy drink
                            "60b0f93284c20f0feb453da7", // Can of RatCola soda
                            "57514643245977207f2c2d09", // Can of TarCola soda
                            "5d1b33a686f7742523398398", // Canister with purified water
                            "60098b1705871270cd5352a1", // Emergency Water Ration
                            "57513f07245977207e26a311", // Pack of apple juice
                            "57513f9324597720a7128161", // Pack of Grand juice
                            "575146b724597720a27126d5", // Pack of milk
                            "544fb62a4bdc2dfb738b4568", // Pack of Russian Army pineapple juice
                            "57513fcc24597720a31c09a6", // Pack of Vita juice
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
            [$"{QuestId} name"] = "Prepper",
            [$"{QuestId} description"] =
                "Well hello there, warrior. I've been thinking about your hideout. Not a bad place, if you know how to make it last. Walls and locks are good, but they don't mean much when your stomach is empty and your throat is dry.\n\n" +
                "If I'm going to start coming through there, I need to know the place can hold out for more than a bad evening. Food, drinks, proper reserves. Enough that a man can sit tight, recover, and not go crawling into the open because he forgot to prepare.\n\n" +
                "Bring me provisions and water. Stock the place properly, and I'll make it one of my stops.\n\n" +
                "** Completing this quest allows Jaeger to be accessible from your hideout **",
            [$"{QuestId} startedMessageText"] =
                "Good. Start with the basics: food and drinks. Found in raid, mind you. A stash is only worth something if you can trust what's inside it.",
            [$"{QuestId} successMessageText"] =
                "You did it? Good work, fella. Now that's a proper reserve. A man can breathe easier when he knows there is food on the shelf and water close by. I'll start using your hideout as one of my stops. Come by when you need me, warrior.",
            [$"{QuestId} failMessageText"] =
                "That's no good. You can't call a place prepared if it can't even feed a tired man.",
            [$"{QuestId} acceptPlayerMessage"] =
                "I understand. I'll stock the place.",
            [$"{QuestId} declinePlayerMessage"] =
                "Not now. I can't help with that.",
            [$"{QuestId} completePlayerMessage"] =
                "Here are the provisions.",
            [$"{QuestId} changeQuestMessageText"] =
                "Keep bringing supplies. Food and water first, everything else later.",
            [RepConditionId] = $"Reach loyalty level {VagabondConfig.Config.JoinHideoutJaegerLoyaltyLevel}",
            [HandoverConditionFoodId] = "Hand over food",
            [HandoverConditionDrinksId] = "Hand over drinks",
        };

        return new Dictionary<string, Dictionary<string, string>>
        {
            ["en"] = en
        };
    }
}