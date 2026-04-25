using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils.Json;
using Vagabond.Server.Config;
using Vagabond.Server.Services;

namespace Vagabond.Server.Data.Quests;

public static class AddRagmanToHideoutQuest
{
    public const string QuestId = "69ec58600000000000000000";
    public const string TraderId = "5ac3b934156ae10c4430e83c";
    private const string HandoverConditionArmorId = "69ec589c0000000000000000";
    private const string HandoverConditionRigsId = "69ec75700000000000000000";
    private const string HandoverConditionBackpacksId = "69ec58d80000000000000000";
    private const string HandoverConditionFabricsId = "69ec59140000000000000000";
    private const string RepConditionId = "69ec59500000000000000000";

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
        var databaseService = ReflectionUtil.GetService<DatabaseService>()!;
        var items = databaseService.GetItems();
        var backpacksIds = items
            .Where(kv => kv.Value.Parent == "5448e53e4bdc2d60728b4567")
            .Where(kv => (kv.Value.Properties?.Grids?.Sum(g =>
                (g.Properties?.CellsH ?? 0) * (g.Properties?.CellsV ?? 0)) ?? 0) >= 20)
            .Select(kv => kv.Key.ToString())
            .ToList();

        var armorIds = items
            .Where(kv => kv.Value.Parent == "5448e54d4bdc2dcc718b4568")
            .Select(kv => kv.Key.ToString())
            .ToList();

        var rigsIds = items
            .Where(kv => kv.Value.Parent == "5448e5284bdc2dcb718b4567")
            .Select(kv => kv.Key.ToString())
            .ToList();

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
                        Value = VagabondConfig.Config.JoinHideoutRagmanLoyaltyLevel,
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
                        Value = VagabondConfig.Config.JoinHideoutRagmanLoyaltyLevel,
                    },
                    new()
                    {
                        Id = HandoverConditionArmorId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        GlobalQuestCounterId = "",
                        Target = new ListOrT<string>(armorIds, null),
                        Value = 15,
                        DogtagLevel = 0,
                        IsEncoded = false,
                        MinDurability = 50,
                        MaxDurability = 100,
                        OnlyFoundInRaid = true,
                        Index = 0,
                        ParentId = "",
                        VisibilityConditions = new List<VisibilityCondition>()
                    },
                    new()
                    {
                        Id = HandoverConditionRigsId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        GlobalQuestCounterId = "",
                        Target = new ListOrT<string>(rigsIds, null),
                        Value = 15,
                        DogtagLevel = 0,
                        IsEncoded = false,
                        MinDurability = 50,
                        MaxDurability = 100,
                        OnlyFoundInRaid = true,
                        Index = 0,
                        ParentId = "",
                        VisibilityConditions = new List<VisibilityCondition>()
                    },
                    new()
                    {
                        Id = HandoverConditionBackpacksId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        GlobalQuestCounterId = "",
                        Target = new ListOrT<string>(backpacksIds, null),
                        Value = 15,
                        DogtagLevel = 0,
                        IsEncoded = false,
                        OnlyFoundInRaid = true,
                        Index = 0,
                        ParentId = "",
                        VisibilityConditions = new List<VisibilityCondition>()
                    },
                    new()
                    {
                        Id = HandoverConditionFabricsId,
                        ConditionType = "HandoverItem",
                        DynamicLocale = false,
                        Value = 20,
                        OnlyFoundInRaid = true,
                        Target = new ListOrT<string>(new List<string>
                        {
                            "5e2af41e86f774755a234b67", // Cordura polyamide fabric
                            "5e2af47786f7746d404f3aaa", // Fleece fabric
                            "5e2af4a786f7746d3f3c3400", // Ripstop fabric
                            "5e2af4d286f7746d4159f07a", // Aramid fiber fabric
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
            [$"{QuestId} name"] = "New Collection",
            [$"{QuestId} description"] =
                "Hey, brother. Come in, take a look. Business is still breathing... barely.\n\n" +
                "I've got clients asking for full kits, armor, rigs, packs, the whole deal. Problem is, my stock is gone. Cleaned out. Either looted or sitting in someone else's stash.\n\n" +
                "Your hideout... that could work. Quiet, controlled. I could run a proper setup there, assemble kits, move product, keep things flowing.\n\n" +
                "But I need inventory first. Body armor, tactical rigs, backpacks, and some fabric to patch, reinforce, finish the work. Everything in good condition, found in raid. No trash.\n\n" +
                "Bring me what I need, and I'll set up shop in your hideout. After that... you'll have access to the good stuff.\n\n" +
                "** Completing this quest allows Ragman to be accessible from your hideout **",
            [$"{QuestId} startedMessageText"] =
                "Armor, rigs, backpacks. That's the core. Fabric too, I use it to fix and finish things. Keep it clean, keep it usable.",
            [$"{QuestId} successMessageText"] =
                "Ohh, now this is a proper batch. Armor's solid, rigs aren't torn to hell, packs still hold weight... yeah, I can work with this.\n\n" +
                "I'll move everything into your hideout and get the line running. Small operation, but efficient. From now on, you come to me there.\n\n" +
                "And trust me... once this starts moving, you'll see the difference.",
            [$"{QuestId} failMessageText"] =
                "Nah, this isn't it. I can't build proper kits out of this junk.",
            [$"{QuestId} changeQuestMessageText"] =
                "Still not enough to run a full line. Bring more gear.",
            [HandoverConditionArmorId] = "Hand over body armor",
            [HandoverConditionRigsId] = "Hand over tactical rigs",
            [HandoverConditionBackpacksId] = "Hand over backpacks",
            [HandoverConditionFabricsId] = "Hand over fabrics",
        };

        return new Dictionary<string, Dictionary<string, string>>
        {
            ["en"] = en
        };
    }
}