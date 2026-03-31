using System;
using System.Linq;
using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using Vagabond.Common.Definitions;

namespace Vagabond.Client.Patches;

internal class TransitInteractionPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(TransitInteractionControllerAbstractClass),
            nameof(TransitInteractionControllerAbstractClass.method_14));
    }

    [PatchPrefix]
    private static bool Prefix(int pointId, Player player)
    {
        if (!CustomExfilPlacementPatch.CustomTransitDefinitions.TryGetValue(pointId, out var definition))
        {
            return true;
        }

        if (MeetsTransitRequirements(player, definition, out var failReason))
        {
            return true;
        }

        NotificationManagerClass.DisplayWarningNotification(string.IsNullOrWhiteSpace(failReason) ? "Requirements not met" : failReason);
        return false;
    }

    private static bool MeetsTransitRequirements(Player player, CustomExfil definition, out string failReason)
    {
        failReason = string.Empty;

        if (definition.Requirements == null || definition.Requirements.Count == 0)
        {
            return true;
        }

        var c = 0;
        foreach (var req in definition.Requirements)
        {
            switch (req.Type)
            {
                case CustomExfilRequirementType.HasItem:
                {
                    var items = player.InventoryController.Inventory.GetPlayerItems(EPlayerItems.Equipment |
                        EPlayerItems.Stash);
                    var count = items.ToList()?.Count(x => x.TemplateId == req.Id);
                    if (count < req.Count)
                    {
                        c++;
                        if (failReason == string.Empty)
                        {
                            failReason = "Missing required item:";
                        }

                        if (!string.IsNullOrWhiteSpace(req.RequirementTip))
                        {
                            failReason += $"{(c > 0 ? " & " : " ")}{req.RequirementTip}";
                        }
                    }

                    break;
                }

                case CustomExfilRequirementType.EmptySlot:
                {
                    if (!Enum.TryParse<EquipmentSlot>(req.RequiredSlot, true, out var slot))
                    {
                        failReason = $"Invalid '{req.RequiredSlot}' slot";
                        return false;
                    }

                    var item = player.Equipment.GetSlot(slot).ContainedItem;
                    if (item != null)
                    {
                        failReason = string.IsNullOrWhiteSpace(req.RequirementTip)
                            ? $"{req.RequiredSlot} slot must be empty"
                            : req.RequirementTip;
                        return false;
                    }

                    break;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(failReason))
        {
            return false;
        }

        return true;
    }
}