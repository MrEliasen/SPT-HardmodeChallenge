using System.Collections.Generic;
using System.Reflection;
using EFT;
using EFT.UI.Matchmaker;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace Vagabond.Client.Patches;

internal class MatchMakerLocationFilterPatch : ModulePatch
{
    private static readonly Dictionary<LocationSettingsClass.Location, bool> OriginalEnabled = new();

    private static readonly AccessTools.FieldRef<MatchMakerSelectionLocationScreen, ISession> SessionRef =
        AccessTools.FieldRefAccess<MatchMakerSelectionLocationScreen, ISession>("iSession");

    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(MatchMakerSelectionLocationScreen),
            nameof(MatchMakerSelectionLocationScreen.method_5));
    }

    [PatchPrefix]
    private static void Prefix(MatchMakerSelectionLocationScreen __instance, RaidSettings raidSettings)
    {
        OriginalEnabled.Clear();

        var session = SessionRef(__instance);
        if (session?.LocationSettings?.locations == null)
        {
            return;
        }

        foreach (var location in session.LocationSettings.locations.Values)
        {
            OriginalEnabled[location] = location.Enabled;

            if (ShouldHide(location))
            {
                location.Enabled = false;
            }
        }

        if (raidSettings.SelectedLocation != null && ShouldHide(raidSettings.SelectedLocation))
        {
            raidSettings.SelectedLocation = null;
        }
    }

    [PatchPostfix]
    private static void Postfix()
    {
        foreach (var kvp in OriginalEnabled)
        {
            kvp.Key.Enabled = kvp.Value;
        }

        OriginalEnabled.Clear();
    }

    private static bool ShouldHide(LocationSettingsClass.Location location)
    {
        if (location == null || Vagabond.State.CurrentMap.IsNullOrEmpty())
        {
            return false;
        }

        return Vagabond.State.CurrentMap != location._Id;
    }
}