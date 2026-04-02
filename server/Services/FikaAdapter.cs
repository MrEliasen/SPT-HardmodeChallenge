using System.Reflection;
using SPTarkov.Server.Core.Models.Common;

namespace Vagabond.Server.Services;

public static class FikaAdapter
{
    private static bool _initialized;
    private static bool _available;
    private static object? _headlessService;
    private static PropertyInfo? _headlessClientsProp;

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
        if (headlessServiceType == null)
        {
            return false;
        }

        _headlessService = services.GetService(headlessServiceType);
        if (_headlessService == null)
        {
            return false;
        }

        _headlessClientsProp = headlessServiceType.GetProperty("HeadlessClients");
        _available = _headlessClientsProp != null;
        return _available;
    }

    public static MongoId GetRaidOwnerSessionId(MongoId sessionId)
    {
        if (!_available || _headlessService == null || _headlessClientsProp == null)
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
        if (client == null)
        {
            return sessionId;
        }

        var requesterProp = client.GetType().GetProperty("RequesterSessionID");
        var requesterSessionId = requesterProp?.GetValue(client) as string;

        if (string.IsNullOrWhiteSpace(requesterSessionId))
        {
            return sessionId;
        }

        VagabondLogger.Success($"Raid Owner SessionId: {requesterSessionId}");
        return new MongoId(requesterSessionId);
    }
}