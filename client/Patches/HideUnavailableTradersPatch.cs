using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT;
using EFT.HealthSystem;
using EFT.InventoryLogic;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace Vagabond.Client.Patches;

public class HideUnavailableTraderCardsPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Constructor(
            typeof(TraderScreensGroup.GClass3888),
            new[]
            {
                typeof(TraderClass),
                typeof(IEnumerable<TraderClass>),
                typeof(Profile),
                typeof(InventoryController),
                typeof(IHealthController),
                typeof(AbstractQuestControllerClass),
                typeof(AbstractAchievementControllerClass),
                typeof(ISession)
            });
    }

    [PatchPrefix]
    public static void Prefix(ref TraderClass trader, ref IEnumerable<TraderClass> tradersList)
    {
        if (tradersList == null)
        {
            return;
        }

        var filtered = tradersList.Where(x => x != null && x.Info != null && x.Info.Available).ToArray();
        tradersList = filtered;
        
        if (filtered.Length == 0)
        {
            return;
        }
        
        if (trader == null || trader.Info == null || !trader.Info.Available || !filtered.Contains(trader))
        {
            trader = filtered[0];
        }
    }
}