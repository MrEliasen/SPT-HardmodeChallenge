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
    private ConfigEntry<KeyboardShortcut> _dumpCustomExtractHotkey = null!;
    private ConfigEntry<KeyboardShortcut> _dumpCustomTransitHotkey = null!;
    
    private string _locationDumpPath = null!;
    private string _customExtractDumpPath = null!;
    private string _customTransitDumpPath = null!;

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
        new CustomExfilPlacementPatch().Enable();
        new CustomExfilCleanupPatch().Enable();
        new CustomTransitRetryPatch().Enable();
        new MenuShowPatch().Enable();

        if (IsHeadless())
        {
            Log($"Loaded in headless mode");
            return;
        }

        new HideUnavailableTraderCardsPatch().Enable();
        new SelectAvailableTraderPatch().Enable();
        new TransitInteractionPatch().Enable();
        new SkipInsuranceFlowPatch().Enable();
        NotificationService.Create(transform);

        _dumpHotkey = Config.Bind(
            "Location Capture (Dev)",
            "Dump Location Hotkey",
            new KeyboardShortcut(KeyCode.F8),
            "Press in raid to dump current map, position and yaw to file."
        );

        _dumpCustomExtractHotkey = Config.Bind(
            "Generate Extraction From Location (Dev)",
            "Dump Custom Extract Snippet Hotkey",
            new KeyboardShortcut(KeyCode.F9),
            "Press in raid to dump a copy/paste ready custom extract snippet using the current player position"
        );

        _dumpCustomTransitHotkey = Config.Bind(
            "Generate Transit From Location (Dev)",
            "Dump Custom Transit Snippet Hotkey",
            new KeyboardShortcut(KeyCode.F10),
            "Press in raid to dump a copy/paste ready custom transit snippet using the current player position"
        );

        var pluginDir = Path.GetDirectoryName(Info.Location);
        _locationDumpPath = Path.Combine(pluginDir, "dumped_locations.txt");
        _customExtractDumpPath = Path.Combine(pluginDir, "dumped_custom_extracts.txt");
        _customTransitDumpPath = Path.Combine(pluginDir, "dumped_custom_transits.txt");

        Log("loaded");
    }

    private void Update()
    {
        if (IsHeadless())
        {
            return;
        }

        if (_dumpHotkey.Value.IsDown())
        {
            DumpCurrentLocation();
        }

        if (_dumpCustomExtractHotkey.Value.IsDown())
        {
            DumpCustomExtractDefinition();
        }

        if (_dumpCustomTransitHotkey.Value.IsDown())
        {
            DumpCustomTransitDefinition();
        }
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
            File.AppendAllText(_locationDumpPath, csharpLine + Environment.NewLine);
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

    private void DumpCustomExtractDefinition()
    {
        try
        {
            if (!TryGetCurrentSnapshot(out var snapshot))
            {
                return;
            }

            File.AppendAllText(_customExtractDumpPath, string.Join(Environment.NewLine, new[]
            {
                $"new CustomExfil",
                "{",
                $"    Identifier = \"VGB_EXT_\",",
                $"    DisplayName = \"Human Readable Label\",",
                "    IsTransit = false,",
                "    TemplateExitName = \"\",// only fill if you want a specific template",
                "    EntryPoints = \"\",",
                "    ExfiltrationTime = 20f,",
                $"    X = {snapshot.Position.x:0.###}f,",
                $"    Y = {snapshot.Position.y:0.###}f,",
                $"    Z = {snapshot.Position.z:0.###}f,",
                $"    RotationY = {snapshot.Yaw:0.###}f,",
                "    Side = \"Pmc\"",
                "},;"
            }));
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to dump custom extract snippet: {ex}");
        }
    }

    private void DumpCustomTransitDefinition()
    {
        try
        {
            if (!TryGetCurrentSnapshot(out var snapshot))
            {
                return;
            }

            File.AppendAllText(_customTransitDumpPath, string.Join(Environment.NewLine, new[]
            {
                "new CustomExfil",
                "{",
                $"    Identifier = \"VGB_\",",
                "    IsTransit = true,",
                $"    TransitPointId = 0,// gets auto generated",
                "    DestinationLocation = VagabondLocations.InverseLookupTable[RaidLocation.DESTINATION].First(),",
                "    TargetLocation = VagabondLocations.InverseLookupTable[RaidLocation.DESTINATION].First(),",
                "    Description = \"Transit to \",",
                "    ExfiltrationTime = 5f,",
                "    ActivateAfterSeconds = 0,",
                "    IsActive = true,",
                "    Events = false,",
                "    HideIfNoKey = false,",
                $"    X = {snapshot.Position.x:0.###}f,",
                $"    Y = {snapshot.Position.y:0.###}f,",
                $"    Z = {snapshot.Position.z:0.###}f,",
                $"    RotationY = {snapshot.Yaw:0.###}f,",
                $"    ConnectedIdentifier = \"VGB_\"",
                "},"
            }));
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to dump custom transit snippet: {ex}");
        }
    }

    private static bool TryGetCurrentSnapshot(out LocationSnapshot snapshot)
    {
        snapshot = default;

        var gameWorld = Singleton<GameWorld>.Instance;
        var player = gameWorld?.MainPlayer;

        if (player == null)
        {
            return false;
        }

        snapshot = new LocationSnapshot(
            player.Position,
            player.Transform.eulerAngles.y
        );
        return true;
    }

    private struct LocationSnapshot
    {
        public Vector3 Position;
        public float Yaw;

        public LocationSnapshot(Vector3 position, float yaw)
        {
            Position = position;
            Yaw = yaw;
        }
    }
}