<p align="center">
  <img src="logo.png" alt="Logo" width="400">
</p> 

Vagabond is a SPTarkox gameplay overhaul mod, which aims to turn Tarkov into more of an open world game.

You start with limited money to buy a simple loadout from Fence, with some meds if you are lucky, and your challenge is now to survive. Using only transits to move around Tarko, and specific extracts to get access to traders.

**Heavily inspired by Path To Tarkov, hopefully this mod can help scratch that itch.**
## Install

1. Download the latest version [here](https://github.com/MrEliasen/SPT-Vagabond/releases/latest/download/spt-vagabond.zip)
2. extract and copy the `spt-vagabond\server\Vagabond` folder to `SPT\user\mods` and the `spt-vagabond\client\Vagabond` to `bepinex\plugins`.
3. If you use **Headless** clients, you will need to add the client plugin to the headless spt client as well.
4. Create a new profile, and it will get enrolled as a new Vagabond.

## Configuration

Configuration is limited for now, and might always be, create and issue and make a PR if need something.

```json
{
  "PermaDeath": false, // if you die for whatever reason (according to the game), your profile gets wiped.
  "StartingRoubles": 135000, // The amount of money a player starts with
  "EnableFenceChanges": true, // makes fence less random and useful as a starter vendor - recommend removing other traders
  "DisableFlea": true,
  "FixProfiles": false, // will enable SPTs profile fixes "RemoveInvalidTradersFromProfile". Helpful if you used older versions of this mod.
  "StripMailAttachments": true, // still allows mail, but removed all attachments from messages.
  "WipeStashOnFirstRaidEntry": true,
  "DisableFreePickRaidLocation": true, // if disabled, you can choose any map you want when starting a raid.
  "AlsoWipeCarriedMoneyOnFirstRaid": true, // this only works if "WipeStashOnFirstRaidEntry" is enabled
  "AdjustRaidTimeMins": 60, // adjust how long raids last by this amount of minutes, negagive values supported by be careful.
  "IgnoredProfiles": [
    // Enter IDs of profiles you don't want getting affected too much by this mod (not 100% isolation)
  ]
}
```