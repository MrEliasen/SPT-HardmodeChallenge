using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Services.Mod;
using Vagabond.Common.Definitions;

namespace Vagabond.Server.Services;

internal static class VagabondStateService
{
    private const string ModKey = "dev.oogabooga.spt-vagabond";

    public static VagabondSessionState GetState(MongoId sessionId)
    {
        var profileDataService = ReflectionUtil.GetService<ProfileDataService>();
        if (profileDataService == null)
        {
            return new();
        }

        return profileDataService.GetProfileData<VagabondSessionState>(sessionId, ModKey) ?? new VagabondSessionState();
    }

    public static void SaveState(MongoId sessionId, VagabondSessionState state)
    {
        var profileDataService = ReflectionUtil.GetService<ProfileDataService>();
        profileDataService?.SaveProfileData(sessionId, ModKey, state);
    }
}