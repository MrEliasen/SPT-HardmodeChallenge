using System.Reflection;
using ChatShared;
using EFT.UI.Chat;
using HarmonyLib;
using SPT.Reflection.Patching;
using SPT.Reflection.Utils;

namespace Vagabond.Client.Patches;

public class BlockTraderMailClaimGetPatch : ModulePatch
{
    private static readonly AccessTools.FieldRef<MessageView, ChatMessageClass> _messageField =
        AccessTools.FieldRefAccess<MessageView, ChatMessageClass>("DialogueChatMessage");

    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(AttachmentMessageView), "method_4");
    }

    [PatchPrefix]
    public static bool Prefix(AttachmentMessageView __instance)
    {
        var chatMessage = _messageField(__instance);
        var traderId = chatMessage?.Member?.Id;
        if (string.IsNullOrEmpty(traderId))
        {
            return true;
        }

        var profile = ClientAppUtils.GetClientApp()?.Session?.Profile;
        if (profile?.TradersInfo == null)
        {
            return true;
        }

        if (!profile.TradersInfo.TryGetValue(traderId, out var info) || info == null || info.Available)
        {
            return true;
        }

        var name = info.Settings?.Nickname?.Localized() ?? "Trader";
        NotificationManagerClass.DisplayWarningNotification($"{name} is not available at your current location.");
        return false;
    }
}