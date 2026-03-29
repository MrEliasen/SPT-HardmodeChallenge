using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using UnityEngine;
using UnityEngine.Rendering;
using Vagabond.Client.Patches;
using Vagabond.Client.Services;
using Vagabond.Client.State;

namespace Vagabond.Client;
    
[BepInPlugin("dev.oogabooga.spt-vagabond", "Vagabond", BuildInfo.Version)]
public class Vagabond : BaseUnityPlugin
{
    private static ManualLogSource _logger;
    public static VagabondState State { get; private set; } = new();
    private ConfigEntry<KeyboardShortcut> _dumpHotkey = null!;
    private string _LocationDumpPath = null!;
    
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
        new BotSpawnConflictionPatch().Enable();
        
        if (IsHeadless())
        {
            Log($"Loaded in headless mode");
            return;
        }
        
        new MenuShowPatch().Enable();
        new SkipInsuranceScreenPatch().Enable();
        new DisableInsuranceBackNavigationPatch().Enable();
        NotificationService.Create(transform);
        
        _dumpHotkey = Config.Bind(
            "Location Capture (Dev)",
            "Dump Location Hotkey",
            new KeyboardShortcut(KeyCode.F8),
            "Press in raid to dump current map, position and yaw to file."
        );

        var pluginDir = Path.GetDirectoryName(Info.Location);
        _LocationDumpPath = Path.Combine(pluginDir, "dumped_locations.txt");
        
        Log("loaded");
    }

    private void Update()
    {
        if (IsHeadless())
        {
            return;
        }
        
        if (!_dumpHotkey.Value.IsDown())
        {
            return;
        }

        DumpCurrentLocation();
    }

    private void DumpCurrentLocation()
    {
        try
        {
            var gameWorld = Singleton<GameWorld>.Instance;
            var player = gameWorld?.MainPlayer;

            if (player == null)
            {
                return;
            }

            var pos = player.Position;
            var yaw = player.Transform.eulerAngles.y;
            var csharpLine = string.Format(
                "(mapFrom, mapTo) => new ManualSpawnPoint {{ X = {0:0.###}f, Y = {1:0.###}f, Z = {2:0.###}f, Rotation = {3:0.###}f }},",
                pos.x,
                pos.y,
                pos.z,
                yaw
            );
            File.AppendAllText(_LocationDumpPath, csharpLine + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to dump location: {ex}");
        }
    }
    
    public static bool IsHeadless()
    {
        return Application.isBatchMode
               || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
    }
}
