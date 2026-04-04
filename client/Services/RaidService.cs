using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using EFT.UI;
using EFT.UI.BattleTimer;
using HarmonyLib;
using UnityEngine;
using Vagabond.Client.Patches;
using Vagabond.Common.Data;
using Vagabond.Common.Enums;

namespace Vagabond.Client.Services;

public static class RaidService
{
    public static void UpdateCurrentRaidExfils()
    {
        var gameWorld = Singleton<GameWorld>.Instance;
        var game = Singleton<AbstractGame>.Instance;
        var controller = gameWorld?.ExfiltrationController;
        var timerPanel = game?.GameUi?.TimerPanel;

        if (gameWorld == null || game == null || controller == null || timerPanel == null)
        {
            return;
        }

        var locationId = gameWorld.LocationId;
        if (string.IsNullOrWhiteSpace(locationId))
        {
            return;
        }

        var raid = VagabondLocations.NormaliseMapName(locationId);
        if (raid == RaidLocation.Nil)
        {
            return;
        }

        if (!Vagabond.State.CustomExfils.TryGetValue(raid, out var mapExfils) || mapExfils == null)
        {
            return;
        }

        if (!mapExfils.TryGetValue(locationId, out var definitions) || definitions == null || definitions.Count == 0)
        {
            return;
        }

        var before = new HashSet<string>(
            controller.ExfiltrationPoints?
                .Where(x => x?.Settings?.Name != null)
                .Select(x => x.Settings.Name) ??
            Enumerable.Empty<string>(),
            StringComparer.OrdinalIgnoreCase);

        CustomExfilPlacementPatch.ApplyCustomExtracts(
            controller,
            raid,
            definitions.Where(x => !x.IsTransit).ToList(),
            true);

        var livePoints = controller.ExfiltrationPoints?
            .Where(x => x?.Settings?.Name != null && !before.Contains(x.Settings.Name))
            .ToList();

        if (livePoints == null || livePoints.Count == 0)
        {
            return;
        }

        foreach (var point in livePoints)
        {
            RegisterLiveExfilPoint(gameWorld, game, timerPanel, controller, point);
        }

        RefreshPlayerInteractions(gameWorld.MainPlayer);
        timerPanel.ShowTimer(true, true);
    }

    // DISCLAIMER: AI GENERATED CODE START
    private static void RegisterLiveExfilPoint(
        GameWorld gameWorld,
        AbstractGame game,
        ExtractionTimersPanel timerPanel,
        ExfiltrationControllerClass controller,
        ExfiltrationPoint point)
    {
        var timersField = AccessTools.Field(typeof(ExtractionTimersPanel), "dictionary_0");
        var templateField = AccessTools.Field(typeof(ExtractionTimersPanel), "_timerPanelTemplate");
        var containerField = AccessTools.Field(typeof(ExtractionTimersPanel), "_container");
        var sideField = AccessTools.Field(typeof(ExtractionTimersPanel), "eplayerSide_0");
        var stringBuilderField = AccessTools.Field(typeof(ExtractionTimersPanel), "stringBuilder_0");

        var timers = timersField.GetValue(timerPanel) as Dictionary<string, ExitTimerPanel>;
        var template = templateField.GetValue(timerPanel) as ExitTimerPanel;
        var container = containerField.GetValue(timerPanel) as RectTransform;
        var side = sideField.GetValue(timerPanel) is EPlayerSide s ? s : EPlayerSide.Bear;
        var stringBuilder = stringBuilderField.GetValue(timerPanel) as StringBuilder;

        if (timers == null || template == null || container == null || stringBuilder == null)
        {
            return;
        }

        if (!timers.ContainsKey(point.Settings.Name))
        {
            var eligible = controller.EligiblePoints(gameWorld.MainPlayer.Profile);
            var index = Array.FindIndex(
                            eligible,
                            x => string.Equals(x.Settings?.Name, point.Settings?.Name, StringComparison.OrdinalIgnoreCase))
                        + 1;

            if (index <= 0)
            {
                index = timers.Count + 1;
            }

            var row = UnityEngine.Object.Instantiate(template, container);
            row.Show(
                EFTDateTimeClass.UtcNow.AddSeconds(point.Settings.MaxTime * 60f),
                side,
                index,
                stringBuilder,
                true,
                timerPanel.ProfileId,
                point,
                false);

            timers.Add(point.Settings.Name, row);
        }

        BindPointToCurrentGame(game, point);
        game.UpdateExfiltrationUi(point, false, true);
    }

    private static void BindPointToCurrentGame(AbstractGame game, ExfiltrationPoint point)
    {
        var handlerMethod =
            game.GetType().GetMethod(
                "method_9",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new[] { typeof(ExfiltrationPoint), typeof(EExfiltrationStatus) },
                null)
            ??
            game.GetType().GetMethod(
                "method_10",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new[] { typeof(ExfiltrationPoint), typeof(EExfiltrationStatus) },
                null);

        if (handlerMethod == null)
        {
            Vagabond.LogError($"Could not find exfil status handler on {game.GetType().FullName}");
            return;
        }

        var handler = Delegate.CreateDelegate(
            typeof(Action<ExfiltrationPoint, EExfiltrationStatus>),
            game,
            handlerMethod,
            false) as Action<ExfiltrationPoint, EExfiltrationStatus>;

        if (handler == null)
        {
            Vagabond.LogError($"Could not bind exfil status handler on {game.GetType().FullName}");
            return;
        }

        point.OnStatusChanged -= handler;
        point.OnStatusChanged += handler;
    }

    private static void RefreshPlayerInteractions(Player player)
    {
        if (player == null)
        {
            return;
        }

        var owner = player.GetComponent<GamePlayerOwner>();
        if (owner == null)
        {
            return;
        }

        owner.ClearInteractionState();

        try
        {
            owner.InteractionsChangedHandler();
        }
        catch
        {
            // ignore
        }
    }
    // DISCLAIMER: AI GENERATED CODE END
}