namespace Vagabond.Common.Models;

public sealed class CustomExfilDefinition
{
    /// <summary>
    /// Stable internal key for this custom exit. Used for logs, scene object names and save data.
    /// This is not the value EFT matches against the scene exfil at runtime.
    /// </summary>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Human-facing exfil name. This is the value that must end up in ExfiltrationPoint.Settings.Name,
    /// because EFTs exfil controller matches scene objects by Settings.Name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Existing scene exfil used only as a template for the cloned trigger/component setup.
    /// </summary>
    public string TemplateExitName { get; set; } = string.Empty;

    /// <summary>
    /// Comma-separated EFT entry point names allowed to use this exfil.
    /// </summary>
    public string EntryPoints { get; set; } = string.Empty;

    public string Side { get; set; } = "Pmc";
    public float ExfiltrationTime { get; set; } = 20f;
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float RotationY { get; set; }
}
