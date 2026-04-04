using System;
using System.Threading.Tasks;

namespace Vagabond.Client.Services;

public static class CommunicationService
{
    public static async Task RefreshExfilState()
    {
        try
        {
            var resp = await Networking.ApiClient.SyncExfilData();
            Vagabond.State.CustomExfils = resp.CustomExfils;
            
            //CustomExfilPlacementPatch.ApplyCustomExtracts(__instance, raid, definitions.Where(x => !x.IsTransit).ToList());
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
    
    public static async Task RefreshVagabondState()
    {
        try
        {
            var resp = await Networking.ApiClient.SyncVagabondState();
            Vagabond.State.CustomExfils = resp.CustomExfils;

            if (!Vagabond.IsHeadless())
            {
                Vagabond.State.PermaDeath = resp.PermaDeath;
                Vagabond.State.WipeFirstRaid = resp.WipeFirstRaid;
                Vagabond.State.WipeFirstMoney = resp.WipeFirstMoney;
                Vagabond.State.CurrentMap = resp.CurrentMap;
                Vagabond.State.LastRefreshUtc = DateTime.UtcNow;
                Vagabond.State.NewCharacter = resp.NewCharacter;
            }

            // Vagabond.Log($"Loading Custom Extractions");
            // foreach (var raid in Vagabond.State.CustomExfils)
            // {
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