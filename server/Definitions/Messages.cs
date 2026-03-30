using Vagabond.Common.Definitions;
using Vagabond.Common.Enums;
using Vagabond.Server.Config;

namespace Vagabond.Server.Definitions;

public static class Messages
{
    public static string Rules()
    {
        var mesage ="You complete a map by taking a transit exit to another map.\n"+
                 "If you take a v-ex, you go back to your stash, however the map is not counted towards your progression.\n";
        
        if (VagabondConfig._config.WipeStashOnEveryRaidEntry)
        {
            mesage +=
                "\nWhen you enter a raid, all items left in the stash will be deleted. First time you enter a raid, any left over cash is removed.";
        } else if (VagabondConfig._config.WipeStashOnFirstRaidEntry)
        {
            mesage +=
                "\nWhen you enter you your first raid, all items left in the stash will be deleted, including any left over cash.";
        }
        
        mesage += 
            "\nIf you have a secure container, it and anything within, will act as permanent storage and is never wiped.\n";
            
        return mesage;
    }
    
    public static string YouDied()
    {
        return "Because you died, your last location has been reset back to the first raid you entered. Your completion progress remains the same.";
    }

    public static string CompletedChallenge()
    {
        return "Congratulations!\nYou have completed the Vagabond Challenge!\n\nCurrently there is no rewards from completing the challenge, but the 'win' is recorded for the future should it be implemented.";
    }

    public static string WelcomeChallenge()
    {
        return
            "Welcome to the Vagabond Challenge!\n\n" +
            Rules() + "\n\n" +
            MapProgression([]);
    }

    public static string WelcomeOpenWorld()
    {
        return
            "Welcome to the Vagabond!\n" +
            "Your start out with some money and limited access to traders. Once you deploy you will need to scavenge for food and water, move between maps and try to stay alive.\n\n" +
            "Vehicle extracts is the only way to get back to your stash once you are out there.\n\n"+
            "Good Luck!";
    }

    public static string ProfileReset(List<string> completedRaids)
    {
        return
            "Your last Vagabond attempt has been reset, you can now go again.\n\n" +
            Rules() + "\n\n" +
            MapProgression(completedRaids);
    }
    public static string ProfileResetGeneric()
    {
        return
            "Your Vagabond profile has been reset.\n\n" +
            WelcomeOpenWorld();
    }
    

    public static string MapProgression(List<string> completedRaids)
    {
        var progress = "";

        foreach (var raid in LocationData.Locations)
        {
            var completed = completedRaids.Any(x => raid.Value.Contains(x));
            var entry = $"[{(completed ? "X" : " ")}] {raid.Key.ToString()} \n";

            if (!completed)
            {
                if (!VagabondConfig._config.IsLabsRequired && raid.Key == RaidLocation.Labs)
                {
                    continue;
                }

                if (!VagabondConfig._config.IsLabyrinthRequired && raid.Key == RaidLocation.Labyrinth)
                {
                    continue;
                }
            }

            progress += entry;
        }

        return progress;
    }
}