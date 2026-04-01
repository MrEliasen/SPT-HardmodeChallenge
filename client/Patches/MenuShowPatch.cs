using System;
using System.Reflection;
using System.Threading.Tasks;
using EFT;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using Vagabond.Client.Services;
using Vagabond.Common.Data;

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

        if (!Vagabond.State.HasShownWarningMessage && Vagabond.State.NewCharacter)
        {
            var message = Messages.FirstWarning(Vagabond.State.WipeFirstRaid,  Vagabond.State.WipeFirstMoney, Vagabond.State.PermaDeath);
            if (message != "")
            {
                NotificationService.Instance.ShowMessage("New Character Warning!\n" + message);
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
            Vagabond.State.CustomExfils = resp.CustomExfils ?? new();
            
            if (!Vagabond.IsHeadless())
            {
                Vagabond.State.PermaDeath = resp.PermaDeath;
                Vagabond.State.WipeFirstRaid = resp.WipeFirstRaid;
                Vagabond.State.WipeFirstMoney = resp.WipeFirstMoney;
                Vagabond.State.CurrentMap = resp.CurrentMap;
                Vagabond.State.LastRefreshUtc = DateTime.UtcNow;
                Vagabond.State.NewCharacter = resp.NewCharacter;
            }
            
            Vagabond.Log($"Loading Custom Extractions");
            foreach (var raid in Vagabond.State.CustomExfils)
            {
                foreach (var map in raid.Value)
                {
                    foreach (var exfil in map.Value)
                    {
                        var kind = exfil.IsTransit ? "Transit" : "Extract";
                        var desc = exfil.IsTransit ? $"{map.Key} To {exfil.DestinationLocation}" : $" {exfil.DisplayName}";
                        Vagabond.Log($" => [{kind}] {exfil.Identifier} :: {desc}");
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