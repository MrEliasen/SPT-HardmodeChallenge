using System.ComponentModel;

namespace HardmodeChallenge.Server.Models.Enums;

public enum HCLocation
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