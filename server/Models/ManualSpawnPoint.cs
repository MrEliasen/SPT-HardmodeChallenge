namespace Vagabond.Server.Models;

public sealed class ManualSpawnPoint
{
    public required double X { get; init; }
    public required double Y { get; init; }
    public required double Z { get; init; }
    public required double Rotation { get; init; }
}