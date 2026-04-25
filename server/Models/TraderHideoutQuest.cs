using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace Vagabond.Server.Models;

public sealed class TraderHideoutQuest
{
    public string QuestId { get; set; } = "";
    public string QuestName { get; set; } = "";
    public string TraderId { get; set; } = "";
    public List<QuestCondition> Conditions { get; set; } = [];
    public Dictionary<string, Dictionary<string, string>> Locales { get; set; } = new();
}