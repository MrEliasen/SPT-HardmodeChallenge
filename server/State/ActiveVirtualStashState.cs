using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace Vagabond.Server.State;

public sealed class ActiveVirtualStashState
{
    public ActiveVirtualStashState(MongoId sessionId, string traderId, PmcData pmcData)
    {
        SessionId = sessionId;
        TraderId = traderId;
        PmcData = pmcData;
    }

    public MongoId SessionId { get; }
    public string TraderId { get; }
    public PmcData PmcData { get; }
    public int Depth { get; set; } = 1;
    public List<Item> RealItemsSnapshot { get; set; } = new();
    public List<Item> LoadedVirtualItems { get; set; } = new();
}