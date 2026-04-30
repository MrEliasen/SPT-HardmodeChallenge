using SPTarkov.Server.Core.Services.Mod;
using Vagabond.Common.Definitions;

namespace Vagabond.Server.Services;

internal static class StateService
{
    private const string ModKey = "dev.oogabooga.spt-vagabond";

    public static VagabondSessionState GetState(string sessionId)
    {
        var profileDataService = ReflectionUtil.GetService<ProfileDataService>();
        if (profileDataService == null)
        {
            return new();
        }

        return profileDataService.GetProfileData<VagabondSessionState>(sessionId, ModKey) ?? new VagabondSessionState();
    }

    public static void SaveState(string sessionId, VagabondSessionState state)
    {
        var profileDataService = ReflectionUtil.GetService<ProfileDataService>();
        profileDataService?.SaveProfileData(sessionId, ModKey, state);
    }
}