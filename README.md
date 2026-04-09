<p align="center">
  <img src="logo.png" alt="Logo" width="400">
</p> 

Vagabond is a SPTarkov gameplay overhaul mod, which aims to turn Tarkov into an open world game.

You start with limited money to buy a simple loadout from Fence, with some meds if you are lucky, and your challenge is now to survive. Using only transits to move around Tarkov, and specific extracts to get access to traders.

**Heavily inspired by Path To Tarkov, hopefully this mod can help scratch that itch.**

## Known issues / limitations

- Stash is available at every trader
- (Fika) If one of your teammates die and you use a transit after the game will get stuck. I dont think this is a mod issue.

## Install

1. Download the latest version [here](https://github.com/MrEliasen/SPT-Vagabond/releases/latest)
2. extract and copy the `spt-vagabond\server\Vagabond` folder to `SPT\user\mods` and the `spt-vagabond\client\Vagabond` to `bepinex\plugins`.
3. If you use **Headless** clients, you will need to add the client plugin to the headless spt client as well.
4. Create a new profile, and it will get enrolled as a new Vagabond.

## Configuration

Configuration is limited for now, and might always be, create and issue and make a PR if need something.

```json
{
  // if you die for whatever reason (according to the game), your profile gets wiped.
  "PermaDeath": false,
  // The amount of money a player starts with
  "StartingRoubles": 175000,
  // makes fence less random and useful as a starter vendor - recommend removing other traders
  "EnableFenceChanges": true,
  "DisableFlea": true,
  // disables events such as halloween etc.
  "DisableEvents": true,
  // will enable SPTs profile fixes "RemoveInvalidTradersFromProfile". Helpful if you used older versions of this mod.
  // will be removed in a future version.
  "FixProfiles": false,
  // if enabled, will removed all attachments from messages, including rewards
  "StripMailAttachments": false,
  // if enabled, you can choose what map you want to go to and won't be locked to your current location.
  "EnablePickRaidLocation": false,
  // first time you enter a raid, whatever is left over in the stash is wiped.
  "WipeStashOnFirstRaidEntry": true,
  // adjust how long raids last by this amount of minutes, negagive values supported by be careful.
  "AdjustRaidTimeMins": 60,
  // if enabled, you can continue to place the hideout without being told you already have.
  // allowing relocation via an in-game feature will come in a later update.
  "AllowHideoutRelocation": false,
  // Where you will be placed when you die, options are: 
  // "hideout" - will send you to your hideout if you have one, otherwise fence.
  // "fence" - will send you to fence.
  // "stay" - you stay at your last known map/transit
  "OnDeathGoTo": "hideout"
}
```
