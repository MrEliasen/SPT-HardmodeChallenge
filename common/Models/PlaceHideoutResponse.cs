namespace Vagabond.Common.Models;

public class PlaceHideoutResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = "";

    //public CustomExfil? Exfil { get; set; }
    public string? CurrentRaid { get; set; }
    public string? MapName { get; set; }
}