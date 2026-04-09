namespace Vagabond.Common.Data;

public static class ForcedSpawnPointIds
{
    public const string Prefix = "VGB_FORCED_SPAWN_";

    public static bool IsForcedSpawnId(string? spawnPointId)
    {
        return !string.IsNullOrWhiteSpace(spawnPointId) &&
               spawnPointId.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase);
    }

    public static string Build(string locationName, string templateId)
    {
        var safeLocationName = string.IsNullOrWhiteSpace(locationName)
            ? "unknown"
            : locationName.Trim().Replace(' ', '_');

        var safeTemplateId = string.IsNullOrWhiteSpace(templateId)
            ? "template"
            : templateId.Trim().Replace(' ', '_');

        return $"{Prefix}{safeLocationName}_{safeTemplateId}";
    }
}