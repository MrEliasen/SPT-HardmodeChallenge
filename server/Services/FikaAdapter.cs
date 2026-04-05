using System.Reflection;
using SPTarkov.Server.Core.Models.Common;

namespace Vagabond.Server.Services;

public static class FikaAdapter
{
    private static bool _initialized;
    private static bool _available;
    private static object? _headlessService;
    private static PropertyInfo? _headlessClientsProp;
    private static object? _matchService;
    private static MethodInfo? _getMatchIdByProfileMethod;
    private static MethodInfo? _getMatchMethod;

    public static bool Init(IServiceProvider services)
    {
        if (_initialized)
        {
            return _available;
        }

        _initialized = true;

        var fikaAsm = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "FikaServer");

        if (fikaAsm == null)
        {
            return false;
        }

        var headlessServiceType = fikaAsm.GetType("FikaServer.Services.Headless.HeadlessService");
        if (headlessServiceType != null)
        {
            _headlessService = services.GetService(headlessServiceType);
            _headlessClientsProp = headlessServiceType.GetProperty("HeadlessClients");
        }

        var matchServiceType = fikaAsm.GetType("FikaServer.Services.MatchService");
        if (matchServiceType != null)
        {
            _matchService = services.GetService(matchServiceType);
            _getMatchIdByProfileMethod = matchServiceType.GetMethod("GetMatchIdByProfile", [typeof(MongoId)]);
            _getMatchMethod = matchServiceType.GetMethod("GetMatch", [typeof(MongoId?)]);
        }

        _available = _headlessClientsProp != null || (_matchService != null && _getMatchIdByProfileMethod != null);
        return _available;
    }

    public static MongoId GetCanonicalSessionId(MongoId sessionId)
    {
        var matchId = TryGetMatchIdByProfile(sessionId);
        if (matchId.HasValue)
        {
            var match = TryGetMatch(matchId.Value);
            var isHeadless = (bool?)match?.GetType().GetProperty("IsHeadless")?.GetValue(match) ?? false;
            if (isHeadless)
            {
                return GetRaidOwnerSessionId(matchId.Value);
            }

            return matchId.Value;
        }

        return GetRaidOwnerSessionId(sessionId);
    }

    public static MongoId GetRaidOwnerSessionId(MongoId sessionId)
    {
        if (_headlessService == null || _headlessClientsProp == null)
        {
            return sessionId;
        }

        var headlessClients = _headlessClientsProp.GetValue(_headlessService);
        if (headlessClients == null)
        {
            return sessionId;
        }

        var tryGetValue = headlessClients.GetType().GetMethod("TryGetValue");
        if (tryGetValue == null)
        {
            return sessionId;
        }

        var args = new object?[] { sessionId, null };
        var found = (bool)tryGetValue.Invoke(headlessClients, args)!;
        if (!found || args[1] == null)
        {
            return sessionId;
        }

        var client = args[1];
        var requesterProp = client?.GetType().GetProperty("RequesterSessionID");
        var requesterSessionId = requesterProp?.GetValue(client) as string;

        if (string.IsNullOrWhiteSpace(requesterSessionId))
        {
            return sessionId;
        }

        VagabondLogger.Success($"Raid Owner SessionId: {requesterSessionId}");
        return new MongoId(requesterSessionId);
    }

    private static MongoId? TryGetMatchIdByProfile(MongoId sessionId)
    {
        if (_matchService == null || _getMatchIdByProfileMethod == null)
        {
            return null;
        }

        var result = _getMatchIdByProfileMethod.Invoke(_matchService, [sessionId]);
        if (result == null)
        {
            return null;
        }

        return (MongoId)result;
    }

    private static object? TryGetMatch(MongoId matchId)
    {
        if (_matchService == null || _getMatchMethod == null)
        {
            return null;
        }

        return _getMatchMethod.Invoke(_matchService, [matchId]);
    }
}