using System.Reflection;
using HardmodeChallenge.Server.Definitions;
using HardmodeChallenge.Server.Services;
using HardmodeChallenge.Server.State;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Services;

namespace HardmodeChallenge.Server.Patches;

public sealed class ProfileCreatePatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(CreateProfileService).GetMethod(nameof(CreateProfileService.CreateProfile));
    }

    [PatchPostfix]
    public static void Postfix(MongoId sessionId, ref ValueTask<string> __result)
    {
        __result = RunAfterCreate(__result, sessionId);
    }

    private static async ValueTask<string> RunAfterCreate(ValueTask<string> original, MongoId sessionId)
    {
        string profileId = await original.ConfigureAwait(false);
        CreateProfile(sessionId);
        return profileId;
    }

    public static void CreateProfile(MongoId sessionId)
    {
        if (!HardmodeService.ShouldApplyHardmodeRules(sessionId))
        {
            return;
        }

        var pmc = HardmodeService.GetPmcProfile(sessionId);
        if (pmc?.CharacterData?.PmcData == null)
        {
            HardmodeLogger.Error($"BootstrapProfile did not modify profile for {sessionId}; PMC null {sessionId}");
            return;
        }

        var changed = InitializeNewCharacter(sessionId, pmc);
        if (!changed)
        {
            HardmodeLogger.Error(
                $"BootstrapProfile did not modify profile for {sessionId}; InitializeNewCharacter did not complete.");
            return;
        }

        var state = HardmodeState.GetState(sessionId);
        state.ProfileInitialized = true;
        state.ChallengesCompleted = 0; 
        state.CompletedRaids = [];
        HardmodeState.SaveState(sessionId, state);
        HardmodeService.ApplyTraderRestrictions(pmc.CharacterData.PmcData, true);
        HardmodeService.PersistProfileIfPossible(sessionId);
        MailerService.SendMail(sessionId, Messages.Welcome(state.CompletedRaids));

        HardmodeLogger.Success($"activated hardmode profile for {sessionId}.");
    }

    private static bool InitializeNewCharacter(MongoId sessionId, SptProfile pmc)
    {
        if (pmc.CharacterData?.PmcData == null)
        {
            HardmodeLogger.Error("InitializeNewCharacter: PmcData was null.");
            return false;
        }

        var inventory = pmc.CharacterData.PmcData.Inventory;
        if (inventory == null)
        {
            HardmodeLogger.Error("InitializeNewCharacter: inventory was null.");
            return false;
        }

        var items = inventory.Items;
        if (items == null)
        {
            HardmodeLogger.Error("InitializeNewCharacter: inventory items list was null.");
            return false;
        }

        HardmodeService.WipeItems(sessionId, pmc.CharacterData.PmcData, 0, true, true);
        HardmodeService.AddMoney(sessionId, pmc.CharacterData.PmcData);
        return true;
    }
}