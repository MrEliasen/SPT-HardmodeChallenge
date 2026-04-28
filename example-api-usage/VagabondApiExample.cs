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
                // the XYZ coords of the exfil location on the map
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
                // What raid this transit goes to.
                DestinationLocation = VagabondLocations.RaidLocationToMapName(RaidLocation.GroundZero),
                Description = "Transit to Ground Zero",
                ExfiltrationTime = 15f, // 15s transit timer
                IsActive = true,
                // the XYZ coords of the transit location on the map
                X = 653.017f,
                Y = -0.292f,
                Z = -24.815f,
                // and which way to look if this location is used as an infil from another transition
                RotationY = 258.376f
            },
            new CustomExfil
            {
                // The unique Identifier for your exfil.
                // If an exfil exists with this Identifier it will be overwritten by this one
                Identifier = "MYMOD_WOODS_TO_GZ",
                // since its a transit, make sure you designate it as such
                IsTransit = true,
                // What raid this transit goes to.
                DestinationLocation = VagabondLocations.RaidLocationToMapName(RaidLocation.Labs),
                // If you want a player to infil / spawn on a specific extract or transit location,
                // specify the exfils Identifier here.
                // Example: assuming we had another transit or extract we added to GZ
                ConnectedIdentifier = "MYMOD_GZ_TO_WOODS",
                // here we override the need for a labs key to transit to labs via this transit,
                // by specifying a location which does not require a key, like customs.
                // If you leave it blank it will use the Destination's default key (in this case Labs) - vanilla behavior 
                AccessKeysSourceLocation = VagabondLocations.RaidLocationToMapName(RaidLocation.Customs),
                Description = "Transit to Labs",
                ExfiltrationTime = 15f,
                IsActive = true,
                // in case we didn't override AccessKeysSourceLocation, it would require a labs key to use this transit.
                // in which case, if you set this to true, it would hide this transit if you didn't have a labs key on you.
                HideIfNoKey = false,
                // the XYZ coords of the transit location on the map
                X = 653.017f,
                Y = -0.292f,
                Z = -24.815f,
                // and which way to look if this location is used as an infil from another transition
                RotationY = 258.376f
            },
        ];
        
        // here we add the transits and exfils we made, to "Customs"
        Api.AddExfils(RaidLocation.Woods, myCustomTransits, myCustomExfils);
        _logger.Info("Added additional exfils via Vagabond API");
        
        // Now, lets add  the new Fence exfil we made
        Api.AddTraderLocations([
            new TraderLocation
            {
                // The ID of the trader
                TraderId = "579dc571d53a0658a154fbec",
                // the raid we are adding the location to
                Raid = RaidLocation.Woods,
                // the Identifier of the exfil they need to use to access this trader
                ExfilIdentifier = "MYMOD_EXT_FENCE",
            }
        ]);
        _logger.Info("Added additional fence location via Vagabond API");
        
        return Task.CompletedTask;
    }
}