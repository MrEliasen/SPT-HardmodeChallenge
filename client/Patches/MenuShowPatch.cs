using System;
using System.Reflection;
using System.Threading.Tasks;
using EFT;
using EFT.UI;
using HarmonyLib;
using JetBrains.Annotations;
using SPT.Reflection.Patching;
using Vagabond.Client.Services;

namespace Vagabond.Client.Patches;

internal class MenuShowPatch : ModulePatch
{
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
        if (Vagabond.State.ChallengeActive && !Vagabond.State.HasShownWarningMessage && !Vagabond.State.HasEnteredFirstRaid)
        {
            var message = "";

            if (Vagabond.State.WipeEveryRaid)
            {
                message += "\nFirst time you enter a raid, any money you carry on you is removed.\n";
                message += "Everything left in your stash is wiped every time you enter a raid. Carry with you what you want to keep.\n";
            }
            else if (Vagabond.State.WipeFirstRaid)
            {
                message += "\nFirst time you enter a raid, any money you carry on you is removed, and everything left in your stash gets wiped\n";
            }

            if (Vagabond.State.LooseAccessToTraders)
            {
                message += "\nOnce you enter your first raid, you will loose access to some of the traders.";
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
   
    [CanBeNull]
    private static async Task RefreshVagabondState()
    {
        try
        {
            var resp = await Networking.ApiClient.HydrateVagabondState();
            Vagabond.State.ChallengeActive = resp.ChallengeActive;
            Vagabond.State.HasEnteredFirstRaid = resp.HasEnteredFirstRaid;
            Vagabond.State.CompletedRaids = resp.CompletedRaids;
            Vagabond.State.LastRefreshUtc = DateTime.UtcNow;
            Vagabond.State.WipeEveryRaid = resp.WipeEveryRaid;
            Vagabond.State.WipeFirstRaid = resp.WipeFirstRaid;
            Vagabond.State.LooseAccessToTraders = resp.LooseAccessToTraders;
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