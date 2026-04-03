using SPTarkov.Server.Core.Models.Utils;

namespace Vagabond.Server.Models;

public sealed class PlaceHideoutServerRequest : IRequestData
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float R { get; set; }
    public string? LocationId { get; set; }
}