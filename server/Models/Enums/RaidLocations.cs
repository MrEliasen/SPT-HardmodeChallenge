using System.ComponentModel;

namespace Vagabond.Server.Models.Enums;

public enum RaidLocations
{
    Nil,
    Factory,
    [Description("Ground Zero")] GroundZero,
    Streets,
    Woods,
    Customs,
    Interchange,
    Lighthouse,
    Reserve,
    Shoreline,
    Labs,
    Labyrinth
}