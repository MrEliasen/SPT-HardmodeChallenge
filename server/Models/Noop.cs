namespace Vagabond.Server.Models;

public sealed class Noop : IDisposable
{
    public static readonly Noop Instance = new();

    public void Dispose()
    {
    }
}