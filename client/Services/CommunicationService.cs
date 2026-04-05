using System;
using System.Threading.Tasks;
using Vagabond.Common.Models;

namespace Vagabond.Client.Services;

public static class CommunicationService
{
    public static async Task RefreshExfilState()
    {
        try
        {
            var resp = await Networking.ApiClient.SyncExfilData(new GetExfilDataRequest
            {
                Version = Vagabond.State.CustomExfilsCacheVersion,
            });

            Vagabond.Log($"RefreshExfilState: version={resp.Version}");
            Vagabond.State.CustomExfilsCacheVersion = resp.Version;

            if (resp.CustomExfils != null)
            {
                Vagabond.Log($"RefreshExfilState: updating exfils");
                Vagabond.State.CustomExfils = resp.CustomExfils;
                RaidService.UpdateCurrentRaidExfils();
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

    public static async Task RefreshVagabondState()
    {
        try
        {
            var resp = await Networking.ApiClient.SyncVagabondState();
            Vagabond.Log($"Loading Custom Extractions");
            Vagabond.State.CustomExfils = resp.CustomExfils;

            foreach (var raid in Vagabond.State.CustomExfils)
            {
                Vagabond.Log($" === {raid.Key} ===");
                foreach (var map in raid.Value)
                {
                    foreach (var exfil in map.Value)
                    {
                        var kind = exfil.IsTransit ? "Transit" : "Extract";
                        var desc = exfil.IsTransit
                            ? $"{map.Key} To {exfil.DestinationLocation}"
                            : $" {exfil.DisplayName}";
                        Vagabond.Log($" => [{kind}] {exfil.Identifier} :: {desc}");
                    }
                }
            }

            if (!Vagabond.IsHeadless())
            {
                //Vagabond.Log($"Loading More information");
                Vagabond.State.PermaDeath = resp.PermaDeath;
                Vagabond.State.WipeFirstRaid = resp.WipeFirstRaid;
                Vagabond.State.WipeFirstMoney = resp.WipeFirstMoney;
                Vagabond.State.CurrentMap = resp.CurrentMap;
                Vagabond.State.LastRefreshUtc = DateTime.UtcNow;
                Vagabond.State.NewCharacter = resp.NewCharacter;
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