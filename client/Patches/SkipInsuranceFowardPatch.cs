using System;
using System.Reflection;
using EFT.UI.Matchmaker;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace Vagabond.Client.Patches
{
    internal class SkipInsuranceScreenPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            var method = typeof(MatchmakerInsuranceScreen).GetMethod(
                "Show",
                BindingFlags.Instance | BindingFlags.Public,
                Type.DefaultBinder,
                new[]
                {
                    typeof(MatchmakerInsuranceScreen.GClass3913)
                },
                null
            );
            
            return method!;
        }

        [PatchPostfix]
        private static void Postfix(MatchmakerInsuranceScreen __instance)
        {
            var screenControllerField = AccessTools.Field(__instance.GetType(), "ScreenController");
            var screenController = screenControllerField?.GetValue(__instance);

            if (screenController == null)
            {
                return;
            }

            var showNextScreen = AccessTools.Method(screenController.GetType(), "ShowNextScreen");
            if (showNextScreen == null)
            {
                return;
            }

            showNextScreen.Invoke(screenController, null);
        }
    }
}