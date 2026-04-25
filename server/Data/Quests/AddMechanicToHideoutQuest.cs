using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Utils.Json;
using Vagabond.Server.Config;

namespace Vagabond.Server.Data.Quests;

public static class AddMechanicToHideoutQuest
{
    public const string QuestId = "69ec12100000000000000000";
    public const string TraderId = "5a7c2eca46aef81a7ca2145d";
    private const string HandoverConditionBatteryId = "69ec124c0000000000000000";
    private const string HandoverConditionToolsId = "69ec12880000000000000000";
    private const string HandoverConditionElectronicsId = "69ec133c0000000000000000";
    private const string HandoverConditionTapeId = "69ec13780000000000000000";
    private const string RepConditionId = "69ec12c40000000000000000";

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
            QuestName = "Setup Shop",
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
                        Value = VagabondConfig.Config.JoinHideoutMechanicLoyaltyLevel,
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
                        Value = VagabondConfig.Config.JoinHideoutMechanicLoyaltyLevel,
                    },
                    new()
                    {
                        Id = HandoverConditionBatteryId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        Value = 10,
                        OnlyFoundInRaid = true,
                        Target = new ListOrT<string>(new List<string>
                        {
                            "5672cb124bdc2d1a0f8b4568", // AA Battery
                            "5672cb304bdc2dc2088b456a", // D Size battery
                            "5733279d245977289b77ec24", // Car battery
                            "590a358486f77429692b2790", // Rechargeable battery
                            "5e2aedd986f7746d404f3aa4", // GreenBat lithium battery
                            "5e2aee0a86f774755a234b62", // Cyclon rechargeable battery
                            "5d03794386f77420415576f5", // 6-STEN-140-M military battery
                        }, null),
                    },
                    new()
                    {
                        Id = HandoverConditionToolsId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        Value = 10,
                        OnlyFoundInRaid = true,
                        Target = new ListOrT<string>(new List<string>
                        {
                            "590c2e1186f77425357b6124", // Toolset
                            "62a0a0bb621468534a797ad5", // Set of files "Master"
                            "590c2d8786f774245b1f03f3", // Screwdriver
                            "5d4042a986f7743185265463", // Flat screwdriver (Long)
                            "5d63d33b86f7746ea9275524", // Flat screwdriver
                            "590c2b4386f77425357b6123", // Pliers
                            "590c2c9c86f774245b1f03f2", // Construction measuring tape
                            "590c311186f77424d1667482", // Wrench
                            "5af04b6486f774195a3ebb49", // Pliers Elite
                            "5d1b317c86f7742523398392", // Hand drill
                            "5d1b31ce86f7742523398394", // Round pliers
                            "5d40419286f774318526545f", // Metal cutting scissors
                            "5d40425986f7743185265461", // Nippers
                            "60391afc25aff57af81f7085", // Ratchet wrench
                            "619cbfccbedcde2f5b3f7bdd", // Pipe grip wrench
                            "619cbfeb6b8a1b37a54eebfa", // Bulbex cable cutter
                            "62a0a098de7ac8199358053b", // Awl
                            "66b37f114410565a8f6789e2", // Inseq gas pipe wrench
                        }, null),
                    },
                    new()
                    {
                        Id = HandoverConditionElectronicsId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        Value = 15,
                        OnlyFoundInRaid = true,
                        Target = new ListOrT<string>(new List<string>
                        {
                            "5672cb724bdc2dc2088b456b", // Geiger-Muller counter
                            "5734779624597737e04bf329", // CPU fan
                            "573477e124597737dd42e191", // CPU
                            "57347c2e24597744902c94a1", // Power supply unit
                            "57347ca924597744596b4e71", // GPU
                            "57347baf24597738002c6178", // RAM stick
                            "590a386e86f77429692b27ab", // Hard drive
                            "590a391c86f774385a33c404", // Magnet
                            "590a3efd86f77437d351a25b", // Gas analyzer
                            "59e36c6f86f774176c10a2a7", // Power cord
                            "5734781f24597737e04bf32a", // DVD drive
                            "5c052f6886f7746b1e3db148", // Military COFDM Wireless Signal Transmitter
                            "5c052fb986f7746b2101e909", // UHF RFID Reader
                            "5c05300686f7746dce784e5d", // VPX Flash Storage Module
                            "5c05308086f7746b2101e90b", // Virtex programmable processor
                            "5c12620d86f7743f8b198b72", // Tetriz portable game console
                            "5d0375ff86f774186372f685", // Military cable
                            "5d0376a486f7747d8050965c", // Military circuit board
                            "5d1b2fa286f77425227d1674", // Electric motor
                            "5d1b304286f774253763a528", // Working LCD
                            "6389c70ca33d8c4cdf4932c6", // Electronic components
                            "6389c7750ef44505c87f5996", // Microcontroller board
                            "6389c7f115805221fb410466", // Far-forward GPS Signal Amplifier Unit
                            "6389c85357baa773a825b356", // Advanced current converter
                            "5bc9b720d4351e450201234b", // Golden 1GPhone smartphone
                            "5c1265fc86f7743f896a21c2", // Broken GPhone X smartphone
                            "56742c324bdc2d150f8b456d", // Broken GPhone smartphone
                        }, null),
                    },
                    new()
                    {
                        Id = HandoverConditionTapeId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        Value = 15,
                        OnlyFoundInRaid = true,
                        Target = new ListOrT<string>(new List<string>
                        {
                            "5734795124597738002c6176", // Insulating tape
                            "57347c1124597737fb1379e3", // Duct tape
                            "5e2af29386f7746d4159f077", // KEKTAPE duct tape
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
            [$"{QuestId} name"] = "Setup Shop",
            [$"{QuestId} description"] =
                "Hello, mercenary. I have a proposition that is less about weapons and more about infrastructure. My current workshop is becoming... inconvenient. Too many eyes, too many interruptions, and not enough control over the equipment. Your hideout has potential. With the right batteries, tools, electronics, and proper insulation, I can set up a small station there. Nothing luxurious. A bench, power, diagnostics, storage, and enough order to make precise work possible. Bring me the supplies, and I'll make myself available from your place when needed.",
            [$"{QuestId} startedMessageText"] =
                "Remember, I need working supplies, not dead weight. Batteries, tools, electronics, tape. Found in raid. If one bad contact ruins the whole bench, we both waste time.",
            [$"{QuestId} successMessageText"] =
                "Good. This will do. Not perfect, but perfection usually requires time, patience, and several boxes of spare parts. I will move the equipment in quietly and set up the station. After that, you can find me in your hideout. Try not to let anyone careless touch my tools.",
            [$"{QuestId} failMessageText"] =
                "No. This will not work. A bad workshop is worse than no workshop at all.",
            [$"{QuestId} acceptPlayerMessage"] =
                "I'll gather what you need.",
            [$"{QuestId} declinePlayerMessage"] =
                "Not now. I have other things to handle.",
            [$"{QuestId} completePlayerMessage"] =
                "Here are the supplies. You can set up shop now.",
            [$"{QuestId} changeQuestMessageText"] =
                "The workstation is not going to assemble itself. Keep bringing the supplies.",
            [RepConditionId] = $"Reach loyalty level {VagabondConfig.Config.JoinHideoutMechanicLoyaltyLevel}",
            [HandoverConditionBatteryId] = "Hand over batteries",
            [HandoverConditionElectronicsId] = "Hand over electronics",
            [HandoverConditionTapeId] = "Hand over tape",
            [HandoverConditionToolsId] = "Hand over tools",
        };

        return new Dictionary<string, Dictionary<string, string>>
        {
            ["en"] = en
        };
    }
}