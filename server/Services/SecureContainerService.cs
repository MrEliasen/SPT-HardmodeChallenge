using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace Vagabond.Server.Services;

public static class SecureContainerService
{
    public static HashSet<string> GetSecureContainerItemIdsToKeep(List<Item> items)
    {
        var keepIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var secureContainer = items.FirstOrDefault(item => item.SlotId == "SecuredContainer");
        if (secureContainer is null)
        {
            return keepIds;
        }

        // Build lookup: parentId -> direct children
        var childrenByParentId = items
            .Where(x => !string.IsNullOrEmpty(x.ParentId))
            .GroupBy(x => x.ParentId)
            .ToDictionary(g => g.Key!, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

        AddItemAndChildrenRecursive(secureContainer.Id, childrenByParentId, keepIds);

        return keepIds;
    }

    private static void AddItemAndChildrenRecursive(
        string itemId,
        Dictionary<string, List<Item>> childrenByParentId,
        HashSet<string> keepIds)
    {
        // already visited
        if (!keepIds.Add(itemId))
        {
            return;
        }

        if (!childrenByParentId.TryGetValue(itemId, out var children))
        {
            return;
        }

        foreach (var child in children)
        {
            AddItemAndChildrenRecursive(child.Id, childrenByParentId, keepIds);
        }
    }
}
