namespace Vagabond.Common.Data;

public static class Messages
{
    public static string WelcomeOpenWorld()
    {
        return
            "Welcome, Vagabond.\n\n" +
            "Your start out with some money and access to Fence. Once you deploy you you won't easily be able to extract.\n" +
            "You will need to worry about food, water and ammunition more than usual, as you will likely stay in raid for much longer.\n\n" +
            "== Your Hideout ==\n\n" +
            "You can place your hideout entrance anywhere in the game, theoretically. Go to where you want your hideout entrance to be and press CTRL+P.\n" +
            "Important: You cannot move your hideout once it is placed (for now) so decide carefully.\n\n" +
            "== Stashes ==\n\n" +
            "Every trader has their own stash separate from your hideout stash. If you use another player's hideout exfil, that also will have its own stash.\n\n" +
            "Good Luck!";
    }

    public static string FirstWarning(bool wipeFirstRaid, bool permadeath)
    {
        var message = "Welcome, Vagabond.\n\n" +
                      "You start out with a bit of money, maybe just enough to buy a ragtag setup from Fence.\n" +
                      "While in a raid, you can press CTRL+P (change via F12) to place the entrance to your hideout to get access to your stash. However once placed you have to pay Skier before you can move it.\n" +
                      "To get traders to be accessible via your hideout, you will need to complete a unique quest for each one. These quests will be available once you reach a certain rep level.\n";

        if (permadeath)
        {
            message +=
                "\nAll stashes, except your hideout stash, is unique to that trader and/or extraction. Some features are disabled in these stashes like sorting.\n";
        }

        if (permadeath)
        {
            message +=
                "\nPerma-Death is enabled. If you die or the game treats you as dead, your equipment and all stashes are wiped and you are reset back to the starter map.\n";
        }

        if (wipeFirstRaid)
        {
            message += $"\nFirst time you enter a raid anything left in this stash will get wiped.";
        }

        return message;
    }
}