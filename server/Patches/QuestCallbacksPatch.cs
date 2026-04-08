using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Callbacks;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.ItemEvent;
using SPTarkov.Server.Core.Models.Eft.Quests;
using Vagabond.Common.Data;
using Vagabond.Server.Services;
using Vagabond.Server.State;

namespace Vagabond.Server.Patches;

public sealed class QuestCallbacksAcceptQuestPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(QuestCallbacks).GetMethod(nameof(QuestCallbacks.AcceptQuest))!;
    }

    [PatchPostfix]
    public static void Postfix(MongoId sessionID, AcceptQuestRequestData info, ItemEventRouterResponse __result)
    {
        if (__result.Warnings != null && __result.Warnings.Count > 0)
        {
            return;
        }

        var state = VagabondState.GetState(sessionID);
        if (ExfilQuests.List.ContainsKey(info.QuestId) && !state.QuestExfils.Contains(info.QuestId))
        {
            state.QuestExfils.Add(info.QuestId);
            VagabondState.SaveState(sessionID, state);
        }
    }
}

public sealed class QuestCallbacksCompleteQuestPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(QuestCallbacks).GetMethod(nameof(QuestCallbacks.CompleteQuest))!;
    }

    [PatchPostfix]
    public static void Postfix(MongoId sessionID, CompleteQuestRequestData info, ItemEventRouterResponse __result)
    {
        if (__result.Warnings != null && __result.Warnings.Count > 0)
        {
            return;
        }

        var state = VagabondState.GetState(sessionID);
        if (state.QuestExfils.Contains(info.QuestId))
        {
            state.QuestExfils.Remove(info.QuestId);
            VagabondState.SaveState(sessionID, state);
        }
    }
}