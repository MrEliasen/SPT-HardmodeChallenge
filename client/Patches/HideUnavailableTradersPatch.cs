using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using EFT.UI;
using SPT.Reflection.Patching;

namespace Vagabond.Client.Patches;

public class HideUnavailableTraderCardsPatch : ModulePatch
{
    private static readonly FieldInfo Cards = AccessTools.Field(typeof(TraderScreensGroup), "list_1");

    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(TraderScreensGroup), nameof(TraderScreensGroup.method_4));
    }

    [PatchPostfix]
    public static void Postfix(TraderScreensGroup __instance)
    {
        var traders = __instance.IEnumerable_0.ToList();
        var cards = (List<TraderCard>)Cards.GetValue(__instance);

        for (var i = 0; i < cards.Count; i++)
        {
            var shouldShow = i < traders.Count && traders[i].Info.Available;
            cards[i].gameObject.SetActive(shouldShow);
        }
    }
}