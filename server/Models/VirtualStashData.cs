using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace Vagabond.Server.Models;

internal sealed class VirtualStashData
{
    public string StashKey { get; set; } = string.Empty;
    public List<Item> Items { get; set; } = new();
}