namespace Vagabond.Common.Definitions;

public sealed class CustomExfil
{
    /// <summary>
    /// Stable internal key for this custom point. Use this for logs, scene object names and save data.
    /// This is not the value EFT matches against the scene exfil at runtime.
    /// </summary>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Human-facing name shown to the player.
    /// For ExfiltrationPoint this must end up in Settings.Name.
    /// For TransitPoint this must end up in parameters.name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// False = custom extract (ExfiltrationPoint), true = custom transit (TransitPoint).
    /// </summary>
    public bool IsTransit { get; set; }

    /// <summary>
    /// Existing scene exfil used only as a template for the cloned trigger/component setup.
    /// </summary>
    public string? TemplateExitName { get; set; } = null;

    /// <summary>
    /// Existing scene transit point id used only as a template for the cloned trigger/component setup.
    /// If null, the first available TransitPoint in the scene is used.
    /// </summary>
    public int? TemplateTransitId { get; set; }

    /// <summary>
    /// Runtime transit point id. Must be unique within the map when IsTransit = true.
    /// </summary>
    public int? TransitPointId { get; set; }

    /// <summary>
    /// EFT map id used by the transit runtime, e.g. bigmap for Customs.
    /// </summary>
    public string DestinationLocation { get; set; } = string.Empty;

    /// <summary>
    /// Example: set this to "laboratory" to require the TerraGroup keycard regardless
    /// of where the transit actually goes there.
    /// Combined with <see cref="HideIfNoKey"/>, the transit is hidden when the key
    /// is absent. If empty, falls back to <see cref="DestinationLocation"/>, meaning
    /// the destination map's own AccessKeys gate the transit (vanilla behavior).
    /// </summary>
    public string AccessKeysSourceLocation { get; set; } = string.Empty;

    /// <summary>
    /// Text shown by the transit interaction UI.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Comma-separated EFT entry point names allowed to use this exfil.
    /// Ignored for TransitPoint.
    /// </summary>
    public string EntryPoints { get; set; } = string.Empty;

    public string Side { get; set; } = "Pmc";
    public float ExfiltrationTime { get; set; } = 20f;
    public int ActivateAfterSeconds { get; set; }
    public bool IsActive { get; set; } = true;
    public bool Events { get; set; }
    public bool HideIfNoKey { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float RotationY { get; set; }
    public string? ConnectedIdentifier { get; set; }

    public bool
        HijackExfil { get; set; } // the exfil of the given template name can only be hijacked by one custom exfil.

    public List<CustomExtractRequirementDefinition> Requirements { get; set; } = new();
}