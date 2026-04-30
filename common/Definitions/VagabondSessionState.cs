namespace Vagabond.Common.Definitions;

public sealed class VagabondSessionState
{
    public bool VagabondModeEnabled { get; set; }
    public bool IsNewCharacter { get; set; }
    public string CurrentMap { get; set; } = "";
    public string LastExit { get; set; } = "";
    public TransitState? TransitState { get; set; }
    public HideoutState? HideoutState { get; set; }
    public HashSet<string> QuestExfils { get; set; } = [];
    public HashSet<string>? RaidFirItems { get; set; }
    public string Version { get; set; } = ModInfo.Version;
    public bool ResetProfile { get; set; }
    public bool CanPlaceHideout { get; set; } = true;
    public HashSet<string> HideoutTraders { get; set; } = [];
}