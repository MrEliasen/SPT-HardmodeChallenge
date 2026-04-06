using System.Reflection;
using System.Text.Json;
using Vagabond.Server.Services;

namespace Vagabond.Server.Config;

public sealed class VagabondConfig
{
    public bool PermaDeath { get; set; }
    public bool FixProfiles { get; set; }
    public bool DisableEvents { get; set; } = true;
    public int StartingRoubles { get; set; } = 175_000;
    public int AdjustRaidTimeMins { get; set; } = 60;
    public bool EnableFenceChanges { get; set; } = true;
    public bool DisableFlea { get; set; } = true;
    public bool StripMailAttachments { get; set; }
    public bool AllowHideoutRelocation { get; set; }
    public bool EnablePickRaidLocation { get; set; }
    public string OnDeathGoTo { get; set; }
    public bool WipeStashOnFirstRaidEntry { get; set; } = true;

    // internal
    public static VagabondConfig Config = new();

    public static void Initialize()
    {
        Config = LoadConfig();
    }

    private static VagabondConfig LoadConfig()
    {
        try
        {
            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                              AppContext.BaseDirectory;
            var localConfig = Path.Combine(assemblyDir, "config", "vagabond.json");
            var siblingConfig = Path.Combine(Directory.GetParent(assemblyDir)?.FullName ?? assemblyDir, "config",
                "vagabond.json");

            var chosen = File.Exists(localConfig) ? localConfig : siblingConfig;
            if (!File.Exists(chosen))
            {
                return new VagabondConfig();
            }

            var json = File.ReadAllText(chosen);
            return JsonSerializer.Deserialize<VagabondConfig>(json, new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                PropertyNameCaseInsensitive = true
            }) ?? new VagabondConfig();
        }
        catch (Exception ex)
        {
            VagabondLogger.Error($"Failed to load config, using defaults: {ex}");
            return new VagabondConfig();
        }
    }
}