using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Api;

/// <summary>
/// Vagabond initialises this during the PreSptModLoader phase.
/// Calling the API before Vagabond's PreSptModLoader phases has run will throw and error.
/// </summary>
public static class Api
{
    internal static Action<RaidLocation, List<CustomExfil>, List<CustomExfil>>? AddExfilsImpl;
    internal static Func<RaidLocation, string, bool>? RemoveExfilImpl;
    internal static Func<RaidLocation, IReadOnlyList<CustomExfil>>? GetExfilsImpl;

    /// <summary>
    /// Add/replace a custom exfils on the given raid.
    /// If a custom exfil with the same id already exists, it is replaced.
    /// </summary>
    /// <param name="raid">Target raid.</param>
    /// <param name="transits">Transit-type exfils (CustomExfil.IsTransit = true).</param>
    /// <param name="extracts">Extract-type exfils (CustomExfil.IsTransit = false).</param>
    public static void AddExfils(RaidLocation raid, List<CustomExfil> transits, List<CustomExfil> extracts)
        => Required(AddExfilsImpl)(raid, transits, extracts);

    /// <summary>
    /// Removes a custom exfil from the given raid by id. Returns true if anything was removed.
    /// </summary>
    public static bool RemoveExfil(RaidLocation raid, string identifier)
        => Required(RemoveExfilImpl)(raid, identifier);

    /// <summary>
    /// Returns the current list of custom exfils for the given raid.
    /// </summary>
    public static IReadOnlyList<CustomExfil> GetExfils(RaidLocation raid)
        => Required(GetExfilsImpl)(raid);

    private static T Required<T>(T? impl) where T : class
        => impl ?? throw new InvalidOperationException(
            "Vagabond is not initialised yet. You must wait until after Vagabond's PreSptModLoader phase.");
}