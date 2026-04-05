using System.Collections.Concurrent;
using SPTarkov.Server.Core.Models.Common;

namespace Vagabond.Server.State;

internal static class RaidRuntimeState
{
    private static readonly ConcurrentDictionary<MongoId, byte> InRaid = new();
    public static void Entered(MongoId sessionId) => InRaid[sessionId] = 1;
    public static void Left(MongoId sessionId) => InRaid.TryRemove(sessionId, out _);
    public static bool IsInRaid(MongoId sessionId) => InRaid.ContainsKey(sessionId);
}