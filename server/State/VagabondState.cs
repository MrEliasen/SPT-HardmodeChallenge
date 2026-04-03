using System.Diagnostics.CodeAnalysis;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Services.Mod;
using Vagabond.Server.Services;

namespace Vagabond.Server.State;

public sealed class VagabondState
{
    private const string ModKey = "dev.oogabooga.spt-vagabond";

    public bool VagabondModeEnabled { get; set; }
    public required string CurrentMap { get; set; }
    public required string LastExit { get; set; }
    public TransitState? TransitState { get; set; }
    public HideoutState? HideoutState { get; set; }
    public bool ResetProfile { get; set; }

    [SetsRequiredMembers]
    public VagabondState()
    {
        CurrentMap = "";
        LastExit = "";
    }

    public static VagabondState GetState(MongoId sessionId)
    {
        var profileDataService = ReflectionUtil.GetService<ProfileDataService>();
        if (profileDataService == null)
        {
            return new VagabondState();
        }

        return profileDataService.GetProfileData<VagabondState>(sessionId, ModKey) ?? new VagabondState();
    }

    public static void SaveState(MongoId sessionId, VagabondState state)
    {
        var profileDataService = ReflectionUtil.GetService<ProfileDataService>();
        profileDataService?.SaveProfileData(sessionId, ModKey, state);
    }
}