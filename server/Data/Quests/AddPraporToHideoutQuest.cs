using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Utils.Json;
using Vagabond.Server.Config;

namespace Vagabond.Server.Data.Quests;

public static class AddPraporToHideoutQuest
{
    public const string QuestId = "69ec04000000000000000000";
    public const string TraderId = "54cb50c76803fa8b248b4571";
    private const string HandoverConditionAmmoId = "69ec04780000000000000000";
    private const string HandoverConditionWeaponsId = "69ec04b40000000000000000";
    private const string HandoverConditionExplosivesId = "69ec04f00000000000000000";
    private const string RepConditionId = "69ec043c0000000000000000";

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
            QuestName = "Supply Lines",
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
                        Value = VagabondConfig.Config.JoinHideoutPraporLoyaltyLevel,
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
                        Value = VagabondConfig.Config.JoinHideoutPraporLoyaltyLevel,
                    },
                    new()
                    {
                        Id = HandoverConditionAmmoId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        Value = 1200,
                        OnlyFoundInRaid = true,
                        Target = new ListOrT<string>(new List<string>
                        {
                            "56dfef82d2720bbd668b4567", // patron_545x39_BP
                            "56dff026d2720bb8668b4567", // patron_545x39_BS
                            "56dff061d2720bb5668b4567", // patron_545x39_BT
                            "56dff0bed2720bb0668b4567", // patron_545x39_FMJ
                            "56dff216d2720bbd668b4568", // patron_545x39_HP
                            "56dff2ced2720bb4668b4567", // patron_545x39_PP
                            "56dff338d2720bbd668b4569", // patron_545x39_PRS
                            "56dff3afd2720bba668b4567", // patron_545x39_PS
                            "56dff421d2720b5f5a8b4567", // patron_545x39_SP
                            "56dff4a2d2720bbd668b456a", // patron_545x39_T
                            "56dff4ecd2720b5f5a8b4568", // patron_545x39_US
                            "5c0d5e4486f77478390952fe", // patron_545x39_BS
                            "61962b617c6c7b169525f168", // patron_545x39_BS
                        }, null),
                    },
                    new()
                    {
                        Id = HandoverConditionExplosivesId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        Value = 10,
                        OnlyFoundInRaid = true,
                        Target = new ListOrT<string>(new List<string>
                        {
                            "5710c24ad2720bc3458b45a3", // F-1 hand grenade
                            "67b49e7335dec48e3e05e057", // F-1 hand grenade (Reduced delay)
                            "5448be9a4bdc2dfd2f8b456a", // RGD-5 hand grenade
                            "617fd91e5539a84ec44ce155", // RGN hand grenade
                            "618a431df1eb8e24b8741deb", // RGO hand grenade
                        }, null),
                    },
                    new()
                    {
                        Id = HandoverConditionWeaponsId,
                        ConditionType = "WeaponAssembly",
                        DynamicLocale = false,
                        Target = new ListOrT<string>(new List<string>
                        {
                            "628b5638ad252a16da6dd245", // SAG AK-545 5.45x39 carbine
                            "628b9c37a733087d0d7fe84b", // SAG AK-545 Short 5.45x39 carbine
                            "5ac66d9b5acfc4001633997a", // Kalashnikov AK-105 5.45x39 assault rifle
                            "6499849fc93611967b034949", // Kalashnikov AK-12 5.45x39 assault rifle
                            "5bf3e03b0db834001d2c4a9c", // Kalashnikov AK-74 5.45x39 assault rifle
                            "5ac4cd105acfc40016339859", // Kalashnikov AK-74M 5.45x39 assault rifle
                            "5644bd2b4bdc2d3b4c8b4572", // Kalashnikov AK-74N 5.45x39 assault rifle
                            "5bf3e0490db83400196199af", // Kalashnikov AKS-74 5.45x39 assault rifle
                            "5ab8e9fcd8ce870019439434", // Kalashnikov AKS-74N 5.45x39 assault rifle
                            "57dc2fa62459775949412633", // Kalashnikov AKS-74U 5.45x39 assault rifle
                            "5839a40f24597726f856b511", // Kalashnikov AKS-74UB 5.45x39 assault rifle
                            "583990e32459771419544dd2", // Kalashnikov AKS-74UN 5.45x39 assault rifle
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
            [$"{QuestId} name"] = "Supply Lines",
            [$"{QuestId} description"] =
                "Hey, warrior. Situation's getting messy. My usual routes are compromised, and the stashes I relied on? Either looted or blown to hell.\n\n" +
                "I've still got people out there, and they need weapons, ammo, something to hold the line. Can't rely on deliveries anymore, so we do it the old way, build our own supply points.\n\n" +
                "Your hideout... not bad. Quiet, tucked away. Could work as a fallback depot. But right now, it's empty, and empty depots don't win fights.\n\n" +
                "Bring me rifles chambered in 5.45x39 with ammo to match, whatever you can spare. Stock it properly, and I'll start routing my business through your place.\n\n" +
                "** Completing this quest allows Prapor to be accessible from your hideout **",
            [$"{QuestId} startedMessageText"] =
                "Don't overthink it. Guns, ammo. Simple. If it shoots, it works. My boys aren't picky, but they do like staying alive.",
            [$"{QuestId} successMessageText"] =
                "Ha! Now that's what I'm talking about. Proper stockpile. With this, we can actually hold something, not just run around like idiots.\n\n" +
                "I'll move some of my operations through your hideout. Safer that way. You need something, you know where to find me.",
            [$"{QuestId} failMessageText"] =
                "No good. This won’t keep anyone alive for long. Bring me something useful.",
            [$"{QuestId} acceptPlayerMessage"] =
                "Got it. I’ll gather weapons and ammo.",
            [$"{QuestId} declinePlayerMessage"] =
                "Not happening. Find someone else.",
            [$"{QuestId} completePlayerMessage"] =
                "Here. Weapons and ammo, like you asked.",
            [$"{QuestId} changeQuestMessageText"] =
                "Still not enough. A depot needs to be stocked, not decorated.",
            [HandoverConditionAmmoId] = "Hand over ammunition",
            [HandoverConditionWeaponsId] = "Hand over weapons",
            [HandoverConditionExplosivesId] = "Hand over explosives",
        };

        return new Dictionary<string, Dictionary<string, string>>
        {
            ["en"] = en
        };
    }
}