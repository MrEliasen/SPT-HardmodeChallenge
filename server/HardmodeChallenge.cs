using System.Reflection;
using HardmodeChallenge.Server.Config;
using HardmodeChallenge.Server.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services.Mod;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Controllers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using Path = System.IO.Path;

namespace HardmodeChallenge.Server;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "dev.oogabooga.spt-hardmodechallenge.server";
    public override string Name { get; init; } = "HardmodeChallenge";
    public override string Author { get; init; } = "Oogabooga.dev";
    public override List<string>? Contributors { get; init; } = new() { "Oogabooga.dev" };
    public override SemanticVersioning.Version Version { get; init; } = new("0.1.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("=4.0.13");
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; } = "https://github.com/mreliasen/spt-hardmodechallenge";
    public override bool? IsBundleMod { get; init; }
    public override string License { get; init; } = "MIT";
}

[Injectable(TypePriority = OnLoadOrder.PreSptModLoader)]
public sealed class HardmodeChallengeLoader : IOnLoad
{
    public ModMetadata MetaData { get; } = new();
    
    public HardmodeChallengeLoader(ISptLogger<HardmodeChallengeLoader> logger)
    {
        HardmodeLogger.Init(logger);
    }

    public Task OnLoad()
    {
        HardmodeConfig.Initialize();

        new Patches.ProfileBootstrapPatch().Enable();;
        new Patches.ProfileCreatePatch().Enable();
        new Patches.RaidEndPatch().Enable();
        new Patches.RaidJoinPatch().Enable();
        new Patches.RaidLocationsPatch().Enable();
        new Patches.MailAttachmentsPatch().Enable();
        return Task.CompletedTask;
    }
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public sealed class HardmodeChallengeDbLoader : IOnLoad
{
    private readonly ProfileDataService _profileDataService;
    private readonly SaveServer _saveServer;
    private readonly InventoryHelper _invHelper;
    private readonly EventOutputHolder _eventOutputHolder;
    private readonly MailSendService _mailSendService;
    private readonly LocationController _locationController;

    private readonly ISptLogger<HardmodeChallengeDbLoader> _logger;

    public HardmodeChallengeDbLoader(
        ProfileDataService profileDataService,
        SaveServer saveServer,
        InventoryHelper invHelper,
        EventOutputHolder eventOutputHolder,
        MailSendService mailSendService,
        LocationController locationController,
        ISptLogger<HardmodeChallengeDbLoader> logger)
    {
        _profileDataService = profileDataService;
        _saveServer = saveServer;
        _logger = logger;
        _invHelper = invHelper;
        _eventOutputHolder = eventOutputHolder;
        _mailSendService = mailSendService;
        _locationController = locationController;
    }

    public Task OnLoad()
    {
        ReflectionUtil.Register(_profileDataService);
        ReflectionUtil.Register(_saveServer);
        ReflectionUtil.Register(_invHelper);
        ReflectionUtil.Register(_eventOutputHolder);
        ReflectionUtil.Register(_mailSendService);
        ReflectionUtil.Register(_locationController);

        _logger.Success($"[HardmodeChallenge] modules loaded.");
        return Task.CompletedTask;
    }
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 2)]
public class BarterTrader(
    ModHelper modHelper,
    ImageRouter imageRouter,
    ConfigServer configServer,
    BarterTraderService barterTraderService,
    DatabaseService databaseService) : IOnLoad
{
    private readonly TraderConfig _traderConfig = configServer.GetConfig<TraderConfig>();

    public Task OnLoad()
    {
        if (!HardmodeConfig._config.AddSpectatorTrader && HardmodeConfig._config.SpectatorTraderAssortment.Count <= 0)
        {
            return Task.CompletedTask;
        }
        
        var assembly = Assembly.GetExecutingAssembly();
        var pathToMod = modHelper.GetAbsolutePathToModFolder(assembly);

        var traderBase = modHelper.GetJsonDataFromFile<TraderBase>(pathToMod, "Assets/trader-base.json");
        var avatarPath = Path.Combine(pathToMod, "Assets/spectator.jpg");
        if (traderBase.Avatar == null)
        {
            return Task.CompletedTask;
        }

        imageRouter.AddRoute(traderBase.Avatar.Replace(".jpg", ""), avatarPath);

        barterTraderService.SetTraderUpdateTime(_traderConfig, traderBase, 1800, 3600);
        barterTraderService.AddTraderWithEmptyAssortToDb(traderBase);
        barterTraderService.AddTraderToLocales(
            traderBase,
            firstName: "Spectator",
            description: "Exchanges merchandise for things to keep you going."
        );

        var trader = databaseService.GetTables().Traders[traderBase.Id];
        barterTraderService.AddTraderAssortment(trader);
        return Task.CompletedTask;
    }
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 2)]
public class DifficultyChanges(DatabaseService databaseService) : IOnLoad
{
    public Task OnLoad()
    {
        if (HardmodeConfig._config.EnableDifficultyChanges)
        {
            var locationsdb = databaseService.GetLocations();
            locationsdb.Sandbox.Base.RequiredPlayerLevelMax = 0;
        }

        if (HardmodeConfig._config.DisableFlea)
        {
            Globals globals = databaseService.GetGlobals();
            globals.Configuration.RagFair.MinUserLevel = 99;
        }
        
        return Task.CompletedTask;
    }
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 5)]
public class FenceTweaks(ConfigServer configServer) : IOnLoad
{
    public Task OnLoad()
    {
        if (!HardmodeConfig._config.EnableFenceChanges)
        {
            return Task.CompletedTask;
        }

        var traderConfig = configServer.GetConfig<TraderConfig>();
        traderConfig.Fence.DiscountOptions.AssortSize = 0;
        traderConfig.Fence.PresetPriceMult = 1.0;
        traderConfig.Fence.ItemPriceMult = 1.0;
        
        // durability
        traderConfig.Fence.WeaponDurabilityPercentMinMax.Max.Min = 90;
        traderConfig.Fence.WeaponDurabilityPercentMinMax.Max.Max = 100;
        traderConfig.Fence.WeaponDurabilityPercentMinMax.Current.Min = 80;
        traderConfig.Fence.WeaponDurabilityPercentMinMax.Current.Max = 95;
        
        traderConfig.Fence.ArmorMaxDurabilityPercentMinMax.Max.Min = 55;
        traderConfig.Fence.ArmorMaxDurabilityPercentMinMax.Max.Max = 85;
        traderConfig.Fence.ArmorMaxDurabilityPercentMinMax.Current.Min = 40;
        traderConfig.Fence.ArmorMaxDurabilityPercentMinMax.Current.Max = 75; 
        
        // refreshing
        traderConfig.Fence.PartialRefreshChangePercent = 0;
        traderConfig.Fence.PartialRefreshTimeSeconds = 600;
        traderConfig.Fence.RegenerateAssortsOnRefresh = false;
        foreach (var update in traderConfig.UpdateTime)
        {
            if (update.TraderId == "579dc571d53a0658a154fbec")
            {
                update.Seconds.Min = 600;
                update.Seconds.Max = 600;
            }
        }
        
        // limits
        traderConfig.Fence.AmmoMaxPenLimit = 28;
        traderConfig.Fence.ItemStackSizeOverrideMinMax[BaseClasses.AMMO] = new MinMax<int>
        {
            Min = 2,
            Max = 10
        };

        traderConfig.Fence.ItemStackSizeOverrideMinMax[BaseClasses.AMMO_BOX] = new MinMax<int>
        {
            Min = 1,
            Max = 10
        };
        
        // pricing
        traderConfig.Fence.ItemCategoryRoublePriceLimit[BaseClasses.AMMO] = 180;
        traderConfig.Fence.ItemCategoryRoublePriceLimit[BaseClasses.WEAPON] = 40000;
        traderConfig.Fence.ItemCategoryRoublePriceLimit[BaseClasses.BACKPACK] = 30000;
        traderConfig.Fence.ItemCategoryRoublePriceLimit[BaseClasses.VEST] = 35000;
        traderConfig.Fence.ItemCategoryRoublePriceLimit[BaseClasses.ARMORED_EQUIPMENT] = 45000;
        traderConfig.Fence.ItemCategoryRoublePriceLimit[BaseClasses.MEDICAL] = 25000;
        traderConfig.Fence.ItemCategoryRoublePriceLimit[BaseClasses.OPTIC_SCOPE] = 20000;
        traderConfig.Fence.ItemCategoryRoublePriceLimit[BaseClasses.SILENCER] = 20000;
        
        // Weapons and ammo
        traderConfig.Fence.ItemTypeLimits[BaseClasses.AMMO] = 140;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.AMMO_BOX] = 100;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.MAGAZINE] = 60;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.PISTOL] = 8;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.SMG] = 4;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.SHOTGUN] = 6;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.ASSAULT_RIFLE] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.ASSAULT_CARBINE] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.MARKSMAN_RIFLE] = 0;
        
        // equipment
        traderConfig.Fence.ItemTypeLimits[BaseClasses.BACKPACK] = 5;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.VEST] = 5;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.ARMORED_EQUIPMENT] = 4;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.HEADWEAR] = 4;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.HEADPHONES] = 1;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.FACE_COVER] = 1;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.ARMOR_PLATE] = 1; 
        
        // misc
        traderConfig.Fence.ItemTypeLimits[BaseClasses.MEDICAL] = 8;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.HANDGUARD] = 1;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.MUZZLE] = 1;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.FLASHLIGHT] = 1;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.FUNCTIONAL_MOD] = 1;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.MOUNT] = 1;
        
        // stuff we dont want to sell
        traderConfig.Fence.ItemTypeLimits[BaseClasses.GEAR_MOD] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.RECEIVER] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.PISTOL_GRIP] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.STOCK] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.IRON_SIGHT] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.MUZZLE_COMBO] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.BARREL] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.BARTER_ITEM] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.COLLIMATOR] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.COMPACT_COLLIMATOR] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.OPTIC_SCOPE] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.ASSAULT_SCOPE] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.SPECIAL_SCOPE] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.SILENCER] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.LIGHT_LASER] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.TACTICAL_COMBO] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.MASTER_MOD] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.AUXILIARY_MOD] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.CHARGE] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.BIPOD] = 0;
        traderConfig.Fence.ItemTypeLimits[BaseClasses.LAUNCHER] = 0;

        //  restocks and presets
        traderConfig.Fence.DiscountOptions.AssortSize = 0;
        traderConfig.Fence.WeaponPresetMinMax.Min = 35;
        traderConfig.Fence.WeaponPresetMinMax.Max = 50;
        traderConfig.Fence.EquipmentPresetMinMax.Min = 3;
        traderConfig.Fence.EquipmentPresetMinMax.Max = 5;
        traderConfig.Fence.AssortSize = 280;

        return Task.CompletedTask;
    }
}
