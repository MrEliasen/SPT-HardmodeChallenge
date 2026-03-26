using System.Reflection;
using System.Text.Json;
using HardmodeChallenge.Server.Services;

namespace HardmodeChallenge.Server.Config;

public sealed class HardmodeConfig
{
    public int StartingRoubles { get; set; } = 150_000;
    public bool EnableFenceChanges { get; set; } = true;
    public bool AddSpectatorTrader { get; set; } = false;
    public bool EnableDifficultyChanges { get; set; } = true;
    public bool DisableFlea { get; set; } = true;
    public bool StripMailAttachments { get; set; } = true;
    public bool AlsoWipeCarriedMoneyOnFirstRaid{ get; set; } = true;
    public bool WipeStashOnEveryRaidEntry { get; set; } = false;
    public bool WipeStashOnFirstRaidEntry { get; set; } = true;
    public bool ResetProfileOnWin { get; set; } = true;
    public bool PreventStarterTraderAccessAfterFirstRaidEntry { get; set; } = true;
    public List<string> StarterTraders { get; set; } = new() {};
    public List<string> PermanentTraders { get; set; } = new()
    {
        "579dc571d53a0658a154fbec"
    };
    public bool IsLabsRequired { get; set; } = true;
    public bool IsLabyrinthRequired { get; set; } = false;
    public List<string> IgnoredProfiles { get; set; } = new() {};
    public Dictionary<string, Dictionary<string, int>> SpectatorTraderAssortment { get; set; } = new();

    // internal
    public static HardmodeConfig _config = new();

    public static void Initialize()
    {
        _config = LoadConfig();
    }

    private static HardmodeConfig LoadConfig()
    {
        try
        {
            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                              AppContext.BaseDirectory;
            var localConfig = Path.Combine(assemblyDir, "config", "hardmode-challenge.json");
            var siblingConfig = Path.Combine(Directory.GetParent(assemblyDir)?.FullName ?? assemblyDir, "config",
                "hardmode-challenge.json");

            var chosen = File.Exists(localConfig) ? localConfig : siblingConfig;
            if (!File.Exists(chosen))
            {
                return new HardmodeConfig();
            }

            var json = File.ReadAllText(chosen);
            return JsonSerializer.Deserialize<HardmodeConfig>(json, new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                PropertyNameCaseInsensitive = true
            }) ?? new HardmodeConfig();
        }
        catch (Exception ex)
        {
            HardmodeLogger.Error($"Failed to load config, using defaults: {ex}");
            return new HardmodeConfig();
        }
    }
}