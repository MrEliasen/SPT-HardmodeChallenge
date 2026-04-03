using System.Linq;
using System.Reflection;
using HarmonyLib;
using EFT.UI;
using SPT.Reflection.Patching;

namespace Vagabond.Client.Patches;

public class SelectAvailableTraderPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(TraderScreensGroup), nameof(TraderScreensGroup.Show));
    }

    [PatchPostfix]
    public static void Postfix(TraderScreensGroup __instance)
    {
        var available = __instance.IEnumerable_0?
            .Where(x => x != null && x.Info != null && x.Info.Available)
            .ToList();

        if (available == null || available.Count == 0)
        {
            return;
        }

        if (!(__instance.TraderClass != null
              && available.Any(x => x.Id == __instance.TraderClass.Id)))
        {
            __instance.method_6(available[0]);
        }
    }
}