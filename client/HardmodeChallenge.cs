using BepInEx;
using BepInEx.Logging;
using HardmodeChallenge.Client.Patches;
using HardmodeChallenge.Client.Services;
using HardmodeChallenge.Client.State;

namespace HardmodeChallenge.Client;
    
[BepInPlugin("dev.oogabooga.spt-hardmodechallenge", "HardmodeChallenge", BuildInfo.Version)]
public class HardmodeChallenge : BaseUnityPlugin
{
    private static ManualLogSource _logger;
    public static HardmodeState State { get; private set; } = new();
    
    public static void Log(string message)
    {
        _logger.LogInfo($"[HardmodeChallenge] {message}");
    }

    public static void LogError(string message)
    {
        _logger.LogError($"[HardmodeChallenge] {message}");
    }

    private void Awake()
    {
        _logger = Logger;
        State = new HardmodeState();
        new ExfiltrationPointPatch().Enable();
        new ExfiltrationPointPatch().Enable();
        new MenuShowPatch().Enable();
        NotificationService.Create(transform);
        Log("loaded");
    }

}
