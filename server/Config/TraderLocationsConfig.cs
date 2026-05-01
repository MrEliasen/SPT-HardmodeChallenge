using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Vagabond.Common.Definitions;
using Vagabond.Server.Services;

namespace Vagabond.Server.Config;

public static class TraderLocationsConfig
{
    public static List<TraderLocation> Locations = new();

    public static void Initialize()
    {
        Locations = LoadConfig();
    }

    private static List<TraderLocation> LoadConfig()
    {
        try
        {
            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                              AppContext.BaseDirectory;
            var localConfig = Path.Combine(assemblyDir, "config", "trader_locations.json");
            var siblingConfig = Path.Combine(Directory.GetParent(assemblyDir)?.FullName ?? assemblyDir, "config",
                "trader_locations.json");

            var chosen = File.Exists(localConfig) ? localConfig : siblingConfig;
            if (!File.Exists(chosen))
            {
                throw new Exception($"trader_locations.json config not found, tried {localConfig} and {siblingConfig}");
            }

            var json = File.ReadAllText(chosen);
            return JsonSerializer.Deserialize<List<TraderLocation>>(json, new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            }) ?? throw new Exception($"failed to read {chosen}");
        }
        catch (Exception ex)
        {
            VagabondLogger.Error($"trader_locations config error, will use empty. Error: {ex}");
            return new List<TraderLocation>();
        }
    }
}