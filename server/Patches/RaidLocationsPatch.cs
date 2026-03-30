using System.Reflection;
using System.Text.Json.Nodes;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Callbacks;
using SPTarkov.Server.Core.Models.Common;
using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;
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
            RaidLocation raidNameE = LocationData.NormaliseMapName(raidName);
            if (raidNameE != RaidLocation.Nil && LocationData.Locations.TryGetValue(raidNameE, out var mapIds))
            {
                foreach (var mapId in mapIds)
                {
                    allowedMapIds.Add(mapId);
                }
            }
        }

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

                if (!IsVehicleExfil(exfil) && !IsCustomExtract(exfil))
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

    private static bool IsCustomExtract(JsonObject exfil)
    {
        string? name = exfil["Name"]?.GetValue<string>();
        string? sptName = exfil["SptName"]?.GetValue<string>();
        return ExfilService.IsCustomExtractName(name) || ExfilService.IsCustomExtractName(sptName);
    }
}
