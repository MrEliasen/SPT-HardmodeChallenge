using System.Collections.Generic;

namespace Vagabond.Client.Models
{
    public class StateResponse
    {
        public bool ChallengeActive { get; set; }
        public bool HasEnteredFirstRaid { get; set; }
        public bool WipeEveryRaid { get; set; }
        public bool WipeFirstRaid { get; set; }
        public bool LooseAccessToTraders { get; set; }
        public List<string> CompletedRaids { get; set; } = new();
    }
}