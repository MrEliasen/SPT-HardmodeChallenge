using System;
using System.Collections.Generic;
using System.Linq;
using Comfort.Common;
using EFT;
using EFT.UI;
using Vagabond.Common.Data;
using Vagabond.Common.Definitions;

namespace Vagabond.Client.Services;

public static class HideoutService
{
    public static void ApplyHideoutExfil(string raidName, string mapName, CustomExfil extract)
    {
        if (raidName == null || mapName == null || extract == null)
        {
            return;
        }

        var gameWorld = Singleton<GameWorld>.Instance;
        if (gameWorld?.ExfiltrationController == null)
        {
            return;
        }

        var raid = VagabondLocations.NormaliseMapName(raidName);

        if (!Vagabond.State.CustomExfils.TryGetValue(raid, out var mapExfils) || mapExfils == null)
        {
            mapExfils = new Dictionary<string, List<CustomExfil>>();
            Vagabond.State.CustomExfils[raid] = mapExfils;
        }

        if (!mapExfils.TryGetValue(mapName, out var list) || list == null)
        {
            list = new List<CustomExfil>();
            mapExfils[mapName] = list;
        }

        if (!list.Any(x => string.Equals(x.Identifier, extract.Identifier, StringComparison.OrdinalIgnoreCase)))
        {
            list.Add(extract);
        }

        RaidService.UpdateCurrentRaidExfils();
        RefreshGameExtractsUi();
    }

    public static void RefreshGameExtractsUi()
    {
        try
        {
            var timerPanel = MonoBehaviourSingleton<GameUI>.Instance?.TimerPanel;
            if (timerPanel == null)
            {
                return;
            }

            timerPanel.ForceUpdateExfiltrationPointsVisitedStatus();
            timerPanel.ShowTimer(true);
        }
        catch (Exception ex)
        {
            Vagabond.LogError($"Failed to refresh extract UI: {ex}");
        }
    }
}