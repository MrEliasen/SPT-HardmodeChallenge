namespace Vagabond.Common.Data;

public static class Messages
{
    public static string WelcomeOpenWorld()
    {
        return
            "Welcome, Vagabond.\n\n" +
            "Your start out with some money and access to Fence. Once you deploy you you won't easily be able to extract.\n" +
            "You will need to worry about food, water and ammunition more than usual, as you will likely stay in raid for much longer.\n\n" +
            "Good Luck!";
    }

    public static string FirstWarning(bool wipeFirstRaid, bool wipeFirstMoney, bool permadeath)
    {
        var message = "Welcome, Vagabond.\n\n" +
                      "Stashes at traders, like the one you are using right now, are unique to that trader and is not your personal permanent stash.";

        if (permadeath)
        {
            message +=
                "\nPerma-Death is enabled. If you die for any reason, your profile is wiped and you are reset back to the start map.\n";
        }

        if (wipeFirstRaid)
        {
            message += $"\nFirst time you enter a raid anything left in this stash will get wiped";

            if (wipeFirstMoney)
            {
                message = $"{message}, including any money you carry on you.";
            }
        }

        return message;
    }
}