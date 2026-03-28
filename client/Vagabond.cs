using BepInEx;
using BepInEx.Logging;
using Vagabond.Client.Patches;
using Vagabond.Client.Services;
using Vagabond.Client.State;

namespace Vagabond.Client;
    
[BepInPlugin("dev.oogabooga.spt-vagabond", "Vagabond", BuildInfo.Version)]
public class Vagabond : BaseUnityPlugin
{
    private static ManualLogSource _logger;
    public static VagabondState State { get; private set; } = new();
    
    public static void Log(string message)
    {
        _logger.LogInfo($"[Vagabond] {message}");
    }

    public static void LogError(string message)
    {
        _logger.LogError($"[Vagabond] {message}");
    }

    private void Awake()
    {
        _logger = Logger;
        State = new VagabondState();
        new ExfiltrationPointPatch().Enable();
        new MatchMakerLocationFilterPatch().Enable();
        new MenuShowPatch().Enable();
        new SkipInsuranceScreenPatch().Enable();
        new DisableInsuranceBackNavigationPatch().Enable();
        
        NotificationService.Create(transform);
        Log("loaded");
    }

}
