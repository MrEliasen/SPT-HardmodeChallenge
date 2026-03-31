using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Models.Spt.Dialog;
using SPTarkov.Server.Core.Services;
using Vagabond.Server.Config;

namespace Vagabond.Server.Patches;

public sealed class MailAttachmentsPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(MailSendService).GetMethod(nameof(MailSendService.SendMessageToPlayer))!;
    }

    [PatchPrefix]
    public static bool Prefix(SendMessageDetails messageDetails)
    {
        if (!VagabondConfig.Config.StripMailAttachments)
        {
            return true;
        }

        // Remove all attached items before SPT converts them into rewards
        messageDetails.Items = [];
        messageDetails.ItemsMaxStorageLifetimeSeconds = null;
        return true;
    }
}