using System.Reflection;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Callbacks;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.ItemEvent;
using SPTarkov.Server.Core.Models.Eft.Quests;
using Vagabond.Common.Data;
using Vagabond.Server.Data.Quests;
using Vagabond.Server.Services;
using Vagabond.Common.Definitions;

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

        var state = VagabondStateService.GetState(sessionID);
        if (ExfilQuests.List.ContainsKey(info.QuestId) && !state.QuestExfils.Contains(info.QuestId))
        {
            state.QuestExfils.Add(info.QuestId);
            VagabondStateService.SaveState(sessionID, state);
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

        var state = VagabondStateService.GetState(sessionID);

        if (state.QuestExfils.Contains(info.QuestId))
        {
            state.QuestExfils.Remove(info.QuestId);
        }

        if (info.QuestId == HideoutRelocationQuest.QuestId)
        {
        }

        switch (info.QuestId)
        {
            case HideoutRelocationQuest.QuestId:
            {
                state.CanPlaceHideout = true;

                var pmc = VagabondService.GetPmcProfile(sessionID)?.CharacterData?.PmcData;
                pmc?.Quests?.RemoveAll(q => q.QId == HideoutRelocationQuest.QuestId);
                break;
            }

            case AddPraporToHideoutQuest.QuestId:
            {
                state.HideoutTraders.Add(AddPraporToHideoutQuest.TraderId);
                break;
            }

            case AddRagmanToHideoutQuest.QuestId:
            {
                state.HideoutTraders.Add(AddRagmanToHideoutQuest.TraderId);
                break;
            }

            case AddJaegerToHideoutQuest.QuestId:
            {
                state.HideoutTraders.Add(AddJaegerToHideoutQuest.TraderId);
                break;
            }

            case AddMechanicToHideoutQuest.QuestId:
            {
                state.HideoutTraders.Add(AddMechanicToHideoutQuest.TraderId);
                break;
            }

            case AddPeacekeeperToHideoutQuest.QuestId:
            {
                state.HideoutTraders.Add(AddPeacekeeperToHideoutQuest.TraderId);
                break;
            }

            case AddSkierToHideoutQuest.QuestId:
            {
                state.HideoutTraders.Add(AddSkierToHideoutQuest.TraderId);
                break;
            }

            case AddTherapistToHideoutQuest.QuestId:
            {
                state.HideoutTraders.Add(AddTherapistToHideoutQuest.TraderId);
                break;
            }
        }

        VagabondStateService.SaveState(sessionID, state);
    }
}