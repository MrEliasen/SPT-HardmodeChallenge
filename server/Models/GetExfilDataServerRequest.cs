using SPTarkov.Server.Core.Models.Utils;

namespace Vagabond.Server.Models;

public sealed class GetExfilDataServerRequest : IRequestData
{
    public int Version { get; set; }
}