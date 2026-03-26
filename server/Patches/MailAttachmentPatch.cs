using System.Reflection;
using HardmodeChallenge.Server.Config;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Models.Spt.Dialog;
using SPTarkov.Server.Core.Services;

namespace HardmodeChallenge.Server.Patches;

public sealed class MailAttachmentsPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(MailSendService).GetMethod(nameof(MailSendService.SendMessageToPlayer))!;
    }

    [PatchPrefix]
    public static bool Prefix(SendMessageDetails messageDetails)
    {
        if (messageDetails is null)
        {
            return true;
        }

        if (HardmodeConfig._config.StripMailAttachments)
        {
            return true;
        }

        // Remove all attached items before SPT converts them into rewards
        messageDetails.Items?.Clear();
        messageDetails.ItemsMaxStorageLifetimeSeconds = null;
        return true;
    }
}