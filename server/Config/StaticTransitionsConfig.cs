using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Vagabond.Common.Enums;
using Vagabond.Common.Models;
using Vagabond.Server.Services;

namespace Vagabond.Server.Config;

public sealed class StaticTransitionEntry
{
    public RaidLocation From { get; set; }
    public RaidLocation To { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public double Rotation { get; set; }
}

public static class StaticTransitionsConfig
{
    private static Dictionary<(RaidLocation, RaidLocation), ManualSpawnPoint> _spawns = new();

    public static void Initialize()
    {
        _spawns = LoadConfig();
    }

    public static ManualSpawnPoint? GetSpawn(RaidLocation from, RaidLocation to)
    {
        return _spawns.TryGetValue((from, to), out var spawn) ? spawn : null;
    }

    private static Dictionary<(RaidLocation, RaidLocation), ManualSpawnPoint> LoadConfig()
    {
        try
        {
            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                              AppContext.BaseDirectory;
            var localConfig = Path.Combine(assemblyDir, "config", "static_transitions.json");
            var siblingConfig = Path.Combine(Directory.GetParent(assemblyDir)?.FullName ?? assemblyDir, "config",
                "static_transitions.json");

            var chosen = File.Exists(localConfig) ? localConfig : siblingConfig;
            if (!File.Exists(chosen))
            {
                throw new Exception(
                    $"static_transitions.json config not found, tried {localConfig} and {siblingConfig}");
            }

            var json = File.ReadAllText(chosen);
            var entries = JsonSerializer.Deserialize<List<StaticTransitionEntry>>(json, new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            }) ?? throw new Exception($"failed to read {chosen}");

            var result = new Dictionary<(RaidLocation, RaidLocation), ManualSpawnPoint>();
            foreach (var entry in entries)
            {
                if (entry.From == RaidLocation.Nil || entry.To == RaidLocation.Nil)
                {
                    VagabondLogger.Warning(
                        $"static_transitions: skipping entry with Nil from/to ({entry.From} -> {entry.To}).");
                    continue;
                }

                result[(entry.From, entry.To)] = new ManualSpawnPoint
                {
                    X = entry.X,
                    Y = entry.Y,
                    Z = entry.Z,
                    Rotation = entry.Rotation
                };
            }

            return result;
        }
        catch (Exception ex)
        {
            VagabondLogger.Error($"static_transitions config error, will use empty. Error: {ex}");
            return new Dictionary<(RaidLocation, RaidLocation), ManualSpawnPoint>();
        }
    }
}