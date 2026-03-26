using System;
using System.Reflection;
using System.Threading.Tasks;
using EFT;
using EFT.UI;
using HardmodeChallenge.Client.Services;
using HarmonyLib;
using JetBrains.Annotations;
using SPT.Reflection.Patching;

namespace HardmodeChallenge.Client.Patches;

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
        if (HardmodeChallenge.State.ChallengeActive && !HardmodeChallenge.State.HasShownWarningMessage && !HardmodeChallenge.State.HasEnteredFirstRaid)
        {
            var message = "";

            if (HardmodeChallenge.State.WipeEveryRaid)
            {
                message += "\nFirst time you enter a raid, any money you carry on you is removed.\n";
                message += "Everything left in your stash is wiped every time you enter a raid. Carry with you what you want to keep.\n";
            }
            else if (HardmodeChallenge.State.WipeFirstRaid)
            {
                message += "\nFirst time you enter a raid, any money you carry on you is removed, and everything left in your stash gets wiped\n";
            }

            if (HardmodeChallenge.State.LooseAccessToTraders)
            {
                message += "\nOnce you enter your first raid, you will loose access to some of the traders.";
            }

            if (message != "")
            {
                NotificationService.Instance.ShowMessage("WARNING\n" + message);
                HardmodeChallenge.State.HasShownWarningMessage = true;
            }
        }

        if (HardmodeChallenge.State.IsRefreshing || (DateTime.UtcNow - HardmodeChallenge.State.LastRefreshUtc).TotalSeconds < 30)
        {
            return;
        }

        HardmodeChallenge.State.IsRefreshing = true;
        _ = RefreshHardmodeState();
    }
   
    [CanBeNull]
    private static async Task RefreshHardmodeState()
    {
        try
        {
            var resp = await Networking.ApiClient.HydrateHardmodeState();
            HardmodeChallenge.State.ChallengeActive = resp.ChallengeActive;
            HardmodeChallenge.State.HasEnteredFirstRaid = resp.HasEnteredFirstRaid;
            HardmodeChallenge.State.CompletedRaids = resp.CompletedRaids;
            HardmodeChallenge.State.LastRefreshUtc = DateTime.UtcNow;
            HardmodeChallenge.State.WipeEveryRaid = resp.WipeEveryRaid;
            HardmodeChallenge.State.WipeFirstRaid = resp.WipeFirstRaid;
            HardmodeChallenge.State.LooseAccessToTraders = resp.LooseAccessToTraders;
        }
        catch (Exception ex)
        {
            HardmodeChallenge.LogError($"Failed to sync hardmode state: {ex}");
        }
        finally
        {
            HardmodeChallenge.State.IsRefreshing = false;
        }
    }
}