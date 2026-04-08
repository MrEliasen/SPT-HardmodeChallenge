using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Services.Mod;
using Vagabond.Server.Services;

namespace Vagabond.Server.State;

public sealed class VagabondState
{
    private const string ModKey = "dev.oogabooga.spt-vagabond";

    public bool VagabondModeEnabled { get; set; }
    public bool IsNewCharacter { get; set; }
    public string CurrentMap { get; set; } = "";
    public string LastExit { get; set; } = "";
    public TransitState? TransitState { get; set; }
    public HideoutState? HideoutState { get; set; }
    public List<string> QuestExfils { get; set; } = [];
    public string Version { get; set; } = "0.3.0";
    public bool ResetProfile { get; set; }

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