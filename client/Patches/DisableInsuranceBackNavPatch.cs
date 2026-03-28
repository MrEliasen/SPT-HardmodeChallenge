using System;
using System.Reflection;
using EFT.UI.Matchmaker;
using SPT.Reflection.Patching;

namespace Vagabond.Client.Patches
{
    internal class DisableInsuranceBackNavigationPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(MatchMakerAcceptScreen).GetMethod(
                "Show",
                BindingFlags.Instance | BindingFlags.Public,
                Type.DefaultBinder,
                new[]
                {
                    typeof(MatchMakerAcceptScreen.GClass3914)
                },
                null
            )!;
        }

        [PatchPostfix]
        private static void Postfix(MatchMakerAcceptScreen __instance)
        {
            __instance.method_9();
        }
    }
}