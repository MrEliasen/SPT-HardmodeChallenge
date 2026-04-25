using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Utils.Json;
using Vagabond.Common.Data;
using Vagabond.Server.Config;

namespace Vagabond.Server.Data.Quests;

public static class AddPeacekeeperToHideoutQuest
{
    public const string QuestId = "69ec20200000000000000000";
    public const string TraderId = "5935c25fb3acc3127c3d8cd9";
    private const string HandoverConditionUsdId = "69ec205c0000000000000000";
    private const string HandoverConditionSuppliesId = "69ec20980000000000000000";
    private const string HandoverConditionSecurityId = "69ec20d40000000000000000";
    private const string RepConditionId = "69ec21100000000000000000";

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
            QuestName = "Forward Logistics",
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
                        Value = VagabondConfig.Config.JoinHideoutPeacekeeperLoyaltyLevel,
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
                        Value = VagabondConfig.Config.JoinHideoutPeacekeeperLoyaltyLevel,
                    },
                    new()
                    {
                        Id = HandoverConditionUsdId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        Value = 5000,
                        OnlyFoundInRaid = false,
                        Target = new ListOrT<string>(new List<string>
                        {
                            Currencies.Dollar,
                        }, null),
                    },
                    new()
                    {
                        Id = HandoverConditionSuppliesId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        Value = 10,
                        OnlyFoundInRaid = true,
                        Target = new ListOrT<string>(new List<string>
                        {
                            "590c5f0d86f77413997acfab", // MRE ration pack
                        }, null),
                    },
                    new()
                    {
                        Id = HandoverConditionSecurityId,
                        ConditionType = "WeaponAssembly",
                        DynamicLocale = false,
                        Target = new ListOrT<string>(new List<string>
                        {
                            "5447a9cd4bdc2dbd208b4567", // Colt M4A1 5.56x45
                            "5c488a752e221602b412af63", // Desert Tech MDR 5.56x45
                            "6184055050224f204c1da540", // FN SCAR-L 5.56x45 assault rifle
                            "618428466ef05c2ce828f218", // FN SCAR-L 5.56x45 assault rifle (FDE)
                            "5bb2475ed4351e00853264e3", // HK 416A5 5.56x45 assault rifle
                            "623063e994fc3f7b302a9696", // HK G36 5.56x45 assault rifle
                            "62e7c4fba689e8c9c50dfc38", // Steyr AUG A1 5.56x45 assault rifle
                            "63171672192e68c5460cebc5", // Steyr AUG A3 5.56x45 assault rifle
                            "6718817435e3cfd9550d2c27", // Steyr AUG A3 5.56x45 assault rifle (Black)
                            "5d43021ca4b9362eab4b5e25", // Lone Star TX-15 DML 5.56x45 carbine
                        }, null),
                        HasItemFromCategory = new List<string>
                        {
                            "5448bc234bdc2d3c308b4569", // Magazine
                            "55818a594bdc2db9688b456a", // Stock
                            "5448fe7a4bdc2d6f028b456b", // Sight
                            "555ef6e44bdc2de9068b457e", // Barrel
                            "55818a104bdc2db9688b4569", // Handguard
                            "55818a684bdc2ddd698b456d", // Pistol grip
                        },
                        Value = 10,
                        Ergonomics = new ValueCompare { CompareMethod = ">=", Value = 10 },
                        Recoil = new ValueCompare { CompareMethod = "<=", Value = 600 },
                        MuzzleVelocity = new ValueCompare { CompareMethod = ">=", Value = 200 },
                        EffectiveDistance = new ValueCompare { CompareMethod = ">=", Value = 200 },
                        BaseAccuracy = new ValueCompare { CompareMethod = "<=", Value = 10 },
                        MagazineCapacity = new ValueCompare { CompareMethod = ">=", Value = 5 },
                        Weight = new ValueCompare { CompareMethod = "<=", Value = 10 },
                        Height = new ValueCompare { CompareMethod = ">=", Value = 2 },
                        Width = new ValueCompare { CompareMethod = ">=", Value = 4 },
                        Durability = new ValueCompare { CompareMethod = ">=", Value = 80 },
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
        var en = new Dictionary<string, string>
        {
            [$"{QuestId} name"] = "Forward Logistics",
            [$"{QuestId} description"] =
                "Good day, my friend. I've been reviewing your hideout, and I must say, it has potential. Not just as a shelter, but as something more... structured.\n\n" +
                "Right now, my operations in Tarkov rely on scattered contacts and unreliable routes. That is inefficient. What I need is a stable forward logistics node. Quiet, controlled, and away from unnecessary attention.\n\n" +
                "Your location fits the criteria. But a proper operation requires preparation. Funding, provisions, and security. Dollars to keep things moving, rations to maintain personnel, and reliable weapons to ensure nothing... interrupts business.\n\n" +
                "You provide these, and I will establish a presence in your hideout. From there, we both benefit.\n\n" +
                "** Completing this quest allows Peacekeeper to be accessible from your hideout **",
            [$"{QuestId} startedMessageText"] =
                "Let's keep this simple. I need three things: money, provisions, and security assets. Dollars are preferable, clean, flexible. MREs will do for supplies. And for security, properly assembled rifles chambered in 5.56x45. Nothing improvised.",
            [$"{QuestId} successMessageText"] =
                "Excellent work. This is exactly what I needed. With these resources, I can establish a functional forward node in your hideout, discreet, efficient, and well supplied.\n\n" +
                "I'll arrange for additional equipment to be moved in quietly. From now on, you'll be able to reach me there. Let's just say... this arrangement stays between us.",
            [$"{QuestId} failMessageText"] =
                "No, this won't work. I can't build a reliable operation on incomplete or substandard supplies.",
            [$"{QuestId} acceptPlayerMessage"] =
                "Understood. I'll gather everything.",
            [$"{QuestId} declinePlayerMessage"] =
                "Not now. I have other priorities.",
            [$"{QuestId} completePlayerMessage"] =
                "Here are the funds, supplies, and equipment.",
            [$"{QuestId} changeQuestMessageText"] =
                "We're not ready yet. Keep the supplies coming.",
            [HandoverConditionUsdId] = "Hand over USD",
            [HandoverConditionSuppliesId] = "Hand over MREs",
            [HandoverConditionSecurityId] = "Hand over rifles",
        };

        return new Dictionary<string, Dictionary<string, string>>
        {
            ["en"] = en
        };
    }
}