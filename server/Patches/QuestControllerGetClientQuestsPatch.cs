using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Controllers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using Vagabond.Server.Data;
using Vagabond.Server.Services;
using Vagabond.Server.State;

namespace Vagabond.Server.Patches;

public sealed class QuestControllerGetClientQuestsPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(QuestController).GetMethod(nameof(QuestController.GetClientQuests))!;
    }

    [PatchPostfix]
    public static void Postfix(MongoId sessionId, ref List<Quest> __result)
    {
        if (__result == null || __result.Count == 0)
        {
            return;
        }

        var state = VagabondState.GetState(sessionId);
        if (state.HideoutState == null)
        {
            return;
        }

        var pmcQuests = VagabondService.GetPmcProfile(sessionId)?.CharacterData?.PmcData?.Quests;
        var isActive = pmcQuests?.Any(q =>
            q.QId == HideoutRelocationQuest.QuestId &&
            (q.Status == QuestStatusEnum.Started ||
             q.Status == QuestStatusEnum.AvailableForFinish)) ?? false;

        if (isActive)
        {
            return;
        }

        __result.RemoveAll(q => q.Id == HideoutRelocationQuest.QuestId);
    }
}