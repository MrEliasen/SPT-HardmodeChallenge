using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;

namespace Vagabond.Common.Api;

/// <summary>
/// Vagabond initialises this during the PreSptModLoader phase.
/// Calling the API before Vagabond's PreSptModLoader phases has run will throw and error.
/// </summary>
public static class Api
{
    // exfils
    internal static Action<RaidLocation, List<CustomExfil>, List<CustomExfil>>? AddExfilsImpl;
    internal static Func<RaidLocation, string, bool>? RemoveExfilImpl;
    internal static Func<RaidLocation, IReadOnlyList<CustomExfil>>? GetExfilsImpl;

    internal static Action<List<TraderLocation>>? AddTraderLocationsImpl;
    internal static Func<string, bool>? RemoveTraderLocationImpl;
    internal static Func<IReadOnlyList<TraderLocation>>? GetTraderLocationsImpl;

    internal static Func<string, VagabondSessionState?>? GetStateImpl;
    internal static Action<string, VagabondSessionState>? SaveStateImpl;

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

    /// <summary>
    /// Add/replace trader location. Entries with an ExfilIdentifier matching an existing one replace it.
    /// </summary>
    public static void AddTraderLocations(List<TraderLocation> extractions)
        => Required(AddTraderLocationsImpl)(extractions);

    /// <summary>
    /// Removes a trader location by ExfilIdentifier. Returns true if removed.
    /// </summary>
    public static bool RemoveTraderExtraction(string exitName)
        => Required(RemoveTraderLocationImpl)(exitName);

    /// <summary>
    /// Returns all currently registered trader locations.
    /// </summary>
    public static IReadOnlyList<TraderLocation> GetTraderExtractions()
        => Required(GetTraderLocationsImpl)();

    /// <summary>
    /// Returns the Vagabond session state for the given profile.
    /// Returns null if the profile does not have one, or if its not yet available.
    /// </summary>
    public static VagabondSessionState? GetState(string sessionId)
        => Required(GetStateImpl)(sessionId);

    /// <summary>
    /// Saves the Vagabond session state for the given profile to disk.
    /// </summary>
    public static void SaveState(string sessionId, VagabondSessionState state)
        => Required(SaveStateImpl)(sessionId, state);

    private static T Required<T>(T? impl) where T : class
        => impl ?? throw new InvalidOperationException(
            "Vagabond is not initialised yet. You must wait until after Vagabond's PreSptModLoader phase.");
}