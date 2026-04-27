using System;
using System.Threading.Tasks;
using Vagabond.Common.Models;

namespace Vagabond.Client.Services;

public static class CommunicationService
{
    public static void RefreshExfilStateBlocking()
    {
        try
        {
            var resp = Networking.ApiClient.SyncExfilDataBlocking(new GetExfilDataRequest
            {
                Version = Vagabond.State.CustomExfilsCacheVersion,
            });

            ApplyExfilState(resp);
        }
        catch (Exception ex)
        {
            Vagabond.LogError($"Failed to synchronously sync Vagabond state: {ex}");
        }
    }

    public static async Task RefreshExfilState()
    {
        try
        {
            var resp = await Networking.ApiClient.SyncExfilData(new GetExfilDataRequest
            {
                Version = Vagabond.State.CustomExfilsCacheVersion,
            });

            ApplyExfilState(resp);
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

    private static void ApplyExfilState(SyncExfilResponse resp)
    {
        if (resp == null)
        {
            return;
        }

        //Vagabond.Log($"RefreshExfilState: version={resp.Version}");
        Vagabond.State.CustomExfilsCacheVersion = resp.Version;

        if (resp.CustomExfils != null)
        {
            //Vagabond.Log($"RefreshExfilState: updating exfils");
            Vagabond.State.CustomExfils = resp.CustomExfils;
            RaidService.UpdateCurrentRaidExfils();
        }
    }

    public static async Task RefreshVagabondState()
    {
        try
        {
            var resp = await Networking.ApiClient.SyncVagabondState();
            ApplyVagabondState(resp);
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

    public static void RefreshVagabondStateBlocking()
    {
        try
        {
            var resp = Networking.ApiClient.SyncVagabondStateBlocking();
            ApplyVagabondState(resp);
        }
        catch (Exception ex)
        {
            Vagabond.LogError($"Failed to synchronously sync Vagabond state: {ex}");
        }
    }

    private static void ApplyVagabondState(SyncStateResponse resp)
    {
        if (resp == null)
        {
            return;
        }

        // foreach (var raid in Vagabond.State.CustomExfils)
        // {
        //     Vagabond.Log($" === {raid.Key} ===");
        //     foreach (var map in raid.Value)
        //     {
        //         foreach (var exfil in map.Value)
        //         {
        //             var kind = exfil.IsTransit ? "Transit" : "Extract";
        //             var desc = exfil.IsTransit
        //                 ? $"{map.Key} To {exfil.DestinationLocation}"
        //                 : $" {exfil.DisplayName}";
        //             Vagabond.Log($" => [{kind}] {exfil.Identifier} :: {desc}");
        //         }
        //     }
        // }

        Vagabond.State.CustomExfils = resp.CustomExfils;
        Vagabond.State.QuestExfils = resp.QuestExfils;
        Vagabond.State.RaidFirItems = resp.RaidFirItems;

        if (!Vagabond.IsHeadless())
        {
            Vagabond.State.ResetOnDeath = resp.ResetOnDeath;
            Vagabond.State.WipeFirstRaid = resp.WipeFirstRaid;
            Vagabond.State.CurrentMap = resp.CurrentMap;
            Vagabond.State.LastRefreshUtc = DateTime.UtcNow;
            Vagabond.State.NewCharacter = resp.NewCharacter;
            Vagabond.State.AllowPostRaidHealing = resp.AllowPostRaidHealing;
            Vagabond.State.LimitTraderMailAccess = resp.LimitTraderMailAccess;
        }
    }
}