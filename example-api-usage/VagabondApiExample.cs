using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using Vagabond.Common.Api;
using Vagabond.Common.Data;
using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;

namespace Vagabond.ApiExample;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "dev.oogabooga.vagabond-api-example";
    public override string Name { get; init; } = "Vagabond API Example";
    public override string Author { get; init; } = "Oogabooga.dev";
    public override SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.13");
    public override string? Url { get; init; } = "https://github.com/MrEliasen/spt-vagabond";
    public override string License { get; init; } = "MIT";
    public override List<string>? Contributors { get; init; } = new() { "Oogabooga.dev" };
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override bool? IsBundleMod { get; init; }
}

// The "+2" makes sure your mod initialises after Vagabond (which is set to +1).
// This is needed for the API to be available for you to use.
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 2)]
public sealed class VagabondApiExampleLoader : IOnLoad
{
    private readonly ISptLogger<VagabondApiExampleLoader> _logger;
    
    public VagabondApiExampleLoader(ISptLogger<VagabondApiExampleLoader> logger)
    {
        _logger = logger;
    }

    public Task OnLoad()
    {
        List<CustomExfil> myCustomExfils = [
            // example of adding a custom extract, which we will hook up to give access to Fence trader.
            new CustomExfil
            {
                // The unique Identifier for your exfil.
                // If an exfil exists with this Identifier it will be overwritten by this one
                Identifier = "MYMOD_EXT_FENCE",
                DisplayName = "Fence's Woods Hideout",
                // only fill if you want this extract to copy a specific exfil's template which already exists in the game
                TemplateExitName = "",
                // if you specify a template name above and want to hijacks its functionality (like D2 bunker on reserver)
                // set this to true, however note an exfil can only be hijacked once across all mods.
                HijackExfil = true,
                // only fill if you want to use specific entry points
                EntryPoints = "",
                // how long extraction timer is 20s in this case
                ExfiltrationTime = 20f,
                // the XYZ coords of the exfil
                X = -122.74f,
                Y = 10.576f,
                Z = -843.164f,
                // and which way to look when spawning back in from this exfil
                RotationY = 90.926f,
                // who can use it, players are Pmc, but maybe you want to do a scav extension or something.
                Side = "Pmc"
            }     
        ];
        
        List<CustomExfil> myCustomTransits = [
            new CustomExfil
            {
                // The unique Identifier for your exfil.
                // If an exfil exists with this Identifier it will be overwritten by this one
                Identifier = "MYMOD_WOODS_TO_GZ",
                // since its a transit, make sure you designate it as such
                IsTransit = true,
                // Specify 
                DestinationLocation = VagabondLocations.RaidLocationToMapName(RaidLocation.Woods),
                TargetLocation = VagabondLocations.RaidLocationToMapName(RaidLocation.GroundZero),
                Description = "Transit to Woods",
                ExfiltrationTime = 15f,
                ActivateAfterSeconds = 0,
                IsActive = true,
                Events = false,
                HideIfNoKey = false,
                X = 653.017f,
                Y = -0.292f,
                Z = -24.815f,
                RotationY = 258.376f
            },
        ];
        
        // the raid to add the transits and exfils to
        Api.AddExfils(RaidLocation.Customs, myCustomTransits, myCustomExfils);
        return Task.CompletedTask;
    }
}