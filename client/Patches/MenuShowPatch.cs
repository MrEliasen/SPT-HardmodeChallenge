using System;
using System.Reflection;
using System.Threading.Tasks;
using EFT;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using Vagabond.Client.Services;

namespace Vagabond.Client.Patches;

internal class MenuShowPatch : ModulePatch
{
    private static bool _headlessUpdated;

    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(
            typeof(MenuScreen),
            "Show",
            new[]
            {
                typeof(Profile),
                typeof(MatchmakerPlayerControllerClass),
                typeof(ESessionMode)
            });
    }

    [PatchPostfix]
    private static void Postfix()
    {
        if (Vagabond.IsHeadless())
        {
            if (!_headlessUpdated)
            {
                _headlessUpdated = true;
                _ = RefreshVagabondState();
            }

            return;
        }

        if (!Vagabond.State.HasShownWarningMessage && Vagabond.State.CurrentMap.IsNullOrEmpty())
        {
            var message = "";

            if (Vagabond.State.PermaDeath)
            {
                message += "\nWARNING: Perma-death is enabled. If you die for any reason, your profile is wiped.\n";
            }
            else if (Vagabond.State.WipeFirstRaid)
            {
                message +=
                    "\nFirst time you enter a raid, any money you carry on you is removed, and everything left in your stash gets wiped\n";
            }

            if (message != "")
            {
                NotificationService.Instance.ShowMessage("Heads up!\n" + message);
                Vagabond.State.HasShownWarningMessage = true;
            }
        }

        if (Vagabond.State.IsRefreshing || (DateTime.UtcNow - Vagabond.State.LastRefreshUtc).TotalSeconds < 30)
        {
            return;
        }

        Vagabond.State.IsRefreshing = true;
        _ = RefreshVagabondState();
    }

    public static async Task RefreshVagabondState()
    {
        try
        {
            var resp = await Networking.ApiClient.HydrateVagabondState();
            Vagabond.State.VagabondModeEnabled = resp.VagabondModeEnabled;
            Vagabond.State.PermaDeath = resp.PermaDeath;
            Vagabond.State.WipeFirstRaid = resp.WipeFirstRaid;
            Vagabond.State.CurrentMap = resp.CurrentMap;
            Vagabond.State.LastRefreshUtc = DateTime.UtcNow;
            Vagabond.State.CustomExfils = resp.CustomExfils ?? new();

            foreach (var raid in Vagabond.State.CustomExfils)
            {
                Vagabond.Log($"Custom extractions for {raid.Key}:");
                foreach (var map in raid.Value)
                {
                    Vagabond.Log($"Specific Map: {map.Key}:");
                    foreach (var exfil in map.Value)
                    {
                        var kind = exfil.IsTransit ? "Transit" : "Extract";
                        var destination = exfil.IsTransit ? $" -> {exfil.DestinationLocation}" : string.Empty;
                        Vagabond.Log($" => [{kind}] {exfil.DisplayName} [{exfil.Identifier}]{destination}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Vagabond.LogError($"Failed to sync Vagabond state: {ex}");
        }
        finally
        {
            Vagabond.State.IsRefreshing = false;
        }
    }
}