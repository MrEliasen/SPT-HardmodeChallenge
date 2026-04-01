using System.Linq;
using System.Reflection;
using HarmonyLib;
using EFT.UI;
using SPT.Reflection.Patching;

namespace Vagabond.Client.Patches;

public class SelectAvailableTraderPatch  : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(TraderScreensGroup), nameof(TraderScreensGroup.Show));
    }

    [PatchPostfix]
    public static void Postfix(TraderScreensGroup __instance)
    {
        if (__instance.TraderClass != null && __instance.TraderClass.Info.Available)
        {
            return;
        }

        var firstAvailable = __instance.IEnumerable_0.FirstOrDefault(x => x.Info.Available);
        if (firstAvailable == null)
        {
            return;
        }

        __instance.method_6(firstAvailable);
    }
}