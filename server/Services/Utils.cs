using System.Reflection;
using SPTarkov.Server.Core.Utils;

namespace Vagabond.Server.Services;

internal static class CopyUtil
{
    private static JsonUtil jsonUtil;
    
    public static void Init(JsonUtil json)
    {
        jsonUtil = json;
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
    
    public static T DeepClone<T>(T source)
    {
        var json = jsonUtil.Serialize(source);
        return jsonUtil.Deserialize<T>(jsonUtil.Serialize(source))!;
    }
    
    public static string? ToJson<T>(T source)
    {
        return jsonUtil.Serialize(source);
    }
}