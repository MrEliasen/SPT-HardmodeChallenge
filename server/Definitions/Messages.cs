
namespace Vagabond.Server.Definitions;

public static class Messages
{
    public static string WelcomeOpenWorld()
    {
        return
            "Welcome to the Vagabond!\n" +
            "Your start out with some money and limited access to traders. Once you deploy you will need to scavenge for food and water, move between maps and try to stay alive.\n\n" +
            "Vehicle extracts is the only way to get back to your stash once you are out there.\n\n"+
            "Good Luck!";
    }

    public static string ProfileResetGeneric()
    {
        return
            "Your Vagabond profile has been reset.\n\n" +
            WelcomeOpenWorld();
    }
}