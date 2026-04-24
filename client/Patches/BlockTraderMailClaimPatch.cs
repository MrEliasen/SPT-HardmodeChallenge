using System.Reflection;
using ChatShared;
using EFT.UI.Chat;
using HarmonyLib;
using SPT.Reflection.Patching;
using SPT.Reflection.Utils;

namespace Vagabond.Client.Patches;

public class BlockTraderMailClaimPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ChatScreen), "method_3", new[] { typeof(DialogueClass) });
    }

    [PatchPrefix]
    public static bool Prefix(DialogueClass dialog)
    {
        if (dialog == null || dialog.Type != EMessageType.NpcTraderMessage)
        {
            return true;
        }

        var profile = ClientAppUtils.GetClientApp()?.Session?.Profile;
        if (profile?.TradersInfo == null)
        {
            return true;
        }

        if (!profile.TradersInfo.TryGetValue(dialog._id, out var info) || info == null || info.Available)
        {
            return true;
        }

        var name = info.Settings?.Nickname?.Localized(null) ?? "Trader";
        NotificationManagerClass.DisplayWarningNotification($"{name} is not available at your current location.");
        return false;
    }
}