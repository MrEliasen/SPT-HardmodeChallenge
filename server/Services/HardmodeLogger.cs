using HardmodeChallenge.Server.Config;
using SPTarkov.Server.Core.Models.Utils;

namespace HardmodeChallenge.Server.Services;

public class HardmodeLogger
{
    private static ISptLogger<HardmodeChallengeLoader>? _logger;
    
    public static void Init(ISptLogger<HardmodeChallengeLoader> logger)
    {
        _logger = logger;
    }
    
    public static void Log(string message)
    {
        _logger?.Success($"[HardmodeChallenge] {message}");
    }

    public static void Error(string message)
    {
        _logger?.Error($"[HardmodeChallenge] {message}");
    }

    public static void Success(string message)
    {
        _logger?.Success($"[HardmodeChallenge] {message}");
    }
}