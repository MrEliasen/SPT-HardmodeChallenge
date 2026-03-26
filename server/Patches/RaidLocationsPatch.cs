using System.Reflection;
using System.Text.Json.Nodes;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Callbacks;
using SPTarkov.Server.Core.Models.Common;
using Vagabond.Server.Definitions;
using Vagabond.Server.Models.Enums;
using Vagabond.Server.Services;
using Vagabond.Server.State;

namespace Vagabond.Server.Patches;

public sealed class RaidLocationsPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(LocationCallbacks).GetMethod(nameof(LocationCallbacks.GetLocationData));
    }

    [PatchPostfix]
    public static void Postfix(MongoId? sessionID, ref ValueTask<string> __result)
    {
        __result = RewriteResponseAsync(sessionID ?? "", __result);
    }

    private static async ValueTask<string> RewriteResponseAsync(MongoId sessionId, ValueTask<string> originalResult)
    {
        string jsonString = await originalResult;

        JsonNode? root = JsonNode.Parse(jsonString);
        if (root is null)
        {
            return jsonString;
        }

        if (!VagabondService.ShouldApplyVagabondRules(sessionId))
        {
            VagabondLogger.Error($"Ignoring for session {sessionId}.");
            return jsonString;
        }

        var pmc = VagabondService.GetPmcProfile(sessionId);
        if (pmc == null || pmc.CharacterData?.PmcData == null)
        {
            VagabondLogger.Error($"Raid extractions: could not resolve PMC profile for {sessionId}.");
            return jsonString;
        }

        var state = VagabondState.GetState(sessionId);
        if (!state.ProfileInitialized)
        {
            VagabondLogger.Error($"Missing state {sessionId}.");
            return jsonString;
        }

        if (state.CompletedRaids.Count == 0)
        {
            VagabondLogger.Error($"CompletedRaids is zero {sessionId}.");
            return jsonString;
        }

        JsonObject? data = root["data"]?.AsObject();
        JsonObject? locations = data?["locations"]?.AsObject();

        if (locations == null)
        {
            VagabondLogger.Error($"locations is null {sessionId}.");
            return jsonString;
        }

        HashSet<string> allowedMapIds = new(StringComparer.OrdinalIgnoreCase);
        foreach (var raidName in state.CompletedRaids)
        {
            RaidLocations raidNameE;
            RaidLocations.TryParse(raidName, true, out raidNameE);

            if (raidNameE != RaidLocations.Nil && LocationData.Locations.TryGetValue(raidNameE, out var mapIds))
            {
                foreach (var mapId in mapIds)
                {
                    allowedMapIds.Add(mapId);
                }
            }
        }

        // remove locations the player has no access to
        foreach (string locationKey in locations.Select(kv => kv.Key).ToList())
        {
            if (!allowedMapIds.Contains(locationKey))
            {
                JsonObject? location = locations[locationKey]?.AsObject();
                if (location != null)
                {
                    location["enabled"] = false;
                }

            }
        }

        // remove all non v-ex
        foreach (string locationKey in locations.Select(x => x.Key).ToList())
        {
            JsonObject? location = locations[locationKey]?.AsObject();
            JsonArray? exits = location?["exits"]?.AsArray();
            bool enabled = location?["enabled"]?.GetValue<bool>() ?? true;

            if (exits is null || !enabled)
            {
                continue;
            }

            for (int i = exits.Count - 1; i >= 0; i--)
            {
                JsonObject? exfil = exits[i]?.AsObject();

                if (exfil is null)
                {
                    exits.RemoveAt(i);
                    continue;
                }

                if (!IsVehicleExfil(exfil))
                {
                    exits.RemoveAt(i);
                }
            }
        }

        return root.ToJsonString(new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = false
        });
    }

    private static bool IsVehicleExfil(JsonObject exfil)
    {
        string? passageRequirement = exfil["PassageRequirement"]?.GetValue<string>();
        string? requirementTip = exfil["RequirementTip"]?.GetValue<string>();
        string? id = exfil["Id"]?.GetValue<string>();

        int count = exfil["Count"]?.GetValue<int>() ?? 0;
        int countPve = exfil["CountPVE"]?.GetValue<int>() ?? 0;

        return string.Equals(passageRequirement, "TransferItem", StringComparison.OrdinalIgnoreCase)
               && string.Equals(id, VagabondService.Roubles, StringComparison.OrdinalIgnoreCase)
               && string.Equals(requirementTip, "EXFIL_Item", StringComparison.OrdinalIgnoreCase)
               && (count > 0 || countPve > 0);
    }
}