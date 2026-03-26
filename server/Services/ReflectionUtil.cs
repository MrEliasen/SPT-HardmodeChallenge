using System.Reflection;
using System.Text.Json;

namespace HardmodeChallenge.Server.Services;

internal static class ReflectionUtil
{
    private static readonly Dictionary<Type, object> ServicesByType = new();
    private static readonly Dictionary<string, object> ServicesByName = new(StringComparer.OrdinalIgnoreCase);

    public static void Register<T>(T service) where T : class
    {
        var type = typeof(T);

        ServicesByType[type] = service;
        RegisterNames(type, service);
    }

    public static T? GetService<T>() where T : class
    {
        return ServicesByType.TryGetValue(typeof(T), out var service)
            ? service as T
            : null;
    }

    private static void RegisterNames(Type type, object service)
    {
        if (!string.IsNullOrWhiteSpace(type.FullName))
        {
            ServicesByName[type.FullName] = service;
        }

        ServicesByName[type.Name] = service;

        foreach (var iface in type.GetInterfaces())
        {
            if (!string.IsNullOrWhiteSpace(iface.FullName))
            {
                ServicesByName[iface.FullName] = service;
            }

            ServicesByName[iface.Name] = service;
        }

        var baseType = type.BaseType;
        while (baseType != null && baseType != typeof(object))
        {
            if (!string.IsNullOrWhiteSpace(baseType.FullName))
            {
                ServicesByName[baseType.FullName] = service;
            }

            ServicesByName[baseType.Name] = service;
            baseType = baseType.BaseType;
        }
    }
    
    private static readonly MethodInfo MemberwiseCloneMethod =
        typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("Could not find MemberwiseClone.");

    public static T ShallowClone<T>(T source) where T : class
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return (T)MemberwiseCloneMethod.Invoke(source, null)!;
    }
}
