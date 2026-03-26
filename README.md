# Hardmode Challenge for SPT 4.0.13+

This is a a mod for SPT 4.0.13+ which adds gameplay changes, a light overhaul.

This mod changes exfil points globally for the hosted raid, which affects both players and AI extraction options.
Currently with this version, that is the quickest and simplest way I could find, without changing this server-side for all players.

I might change this to merely affect actual exfil and leave the markers, but for now, just a heads up.

## What is the hardmode challenge?

1. New profiles are are stripped down to an empty character and stash.
2. Profile is given some amount of roubles to spend before the first raid.
   - The amount of money you are given is configurable.
   - You can choose which traders they have access to before they enter their first raid, and permanently throughout the challenge.
3. On First Raid Entry:
   - It wipes anything still left in the stash and any money you have on you (configurable).
   - It blocks access to all traders only available to you before your first raid (configurable).
   - Choose any starting map, after your first map completion (transit), you can only enter that or any other maps you have transferred out of.
4. Extraction:
   - Normal extracts: Disabled
   - Vehicle extracts: you get back to your stash and permanent traders, but you do not get completion credit for the map.
   - Transition extracts: the map you just extracted from is marked as completed.
5. Once you have successfully gone through all maps and extracted using a v-ex to get back to stash, you complete the challenge
   - Configurable if you want to include labs and labyrinth.
   - Your character and stash is wiped once again (configurable). Only items in your secure container is kept between challenges.

## Features

Just look at the server config, I am lazy.. 

```json
{
   "StartingRoubles": 150000, // The amount of money a player starts with
   "EnableFenceChanges": true, // makes fence less random and useful as a starter vendor - recommend removing other traders
   "WipeStashOnEveryRaidEntry": false,
   "WipeStashOnFirstRaidEntry": true,
   "AlsoWipeCarriedMoneyOnFirstRaid": true, // this only works if one of the other wipe options are enabled
   "DisableFlea": true,
   "ResetProfileOnWin": true, // if you want to play this challenge more as a continuous experience, disable this.
   "StripMailAttachments": true, // still allows mail, but removed all attachments from messages.
   "EnableDifficultyChanges": true, // currently just disabled newbie ground zero.
   "PreventStarterTraderAccessAfterFirstRaidEntry": true,
   "StarterTraders": [
      // add ID's of traders here.
      // access will be removed after player enters first raid, if setting is enabled
      // I recommend trying with just Fence and the fence changes first.
   ],
   "PermanentTraders": [
      // add ID's of traders here.
      // these are permanently available, regardless of how many raids the player has done
      "579dc571d53a0658a154fbec" // fence
   ],
   "IsLabsRequired": false, // if labs is part of the list of required maps to exfil from (untested, assuming there are transfers).
   "IsLabyrinthRequired": false, // as above, assuming you have the mod installed which enables it (untested, assuming there is transfers from that map)
   // warning, this will alter your profiles, even ignored once (below)
   // the new trader id is: "686172646d6f647472616465", if you need to manually remove it from profiles later.
   "IgnoredProfiles": [
      // Enter IDs of profiles you don't want getting affected by this mod
      // Note: trader changes will still affect these profilers, but everything else will be toggled off.
   ],
   "AddSpectatorTrader": true, // its a barter trader, its assortment is below
   // example trader assortment, the idea is you can get secure containers, but you have to
   // trade in the last as part of the next. Adjust what you think should be needed additionally for each.
   // Or add your own stuff.
   "SpectatorTraderAssortment": {
      "544a11ac4bdc2d470e8b456a": { // Secure container alpha
         "59e3658a86f7741776641ac4": 1, // 1x cat figurine
         "5734758f24597738025ee253": 2 // 2x gold chains
      },
      "5857a8b324597729ab0a0e7d": { // Secure container beta
         "544a11ac4bdc2d470e8b456a": 1, // 1x Secure Container alpha
         "5bc9bc53d4351e00367fbcee": 1, // 1x rooster
         "590c5f0d86f77413997acfab": 1 // 4x MREs
      },
      "59db794186f77448bc595262": { // Secure container epsilon
         "5857a8b324597729ab0a0e7d": 1, // 1x Secure Container beta
         "5d403f9186f7743cac3f229b": 1, // 2x Jack D  
         "5b44cad286f77402a54ae7e5": 1 // 1x Tacktical Rig
      },
      "5857a8bc2459772bad15db29": { // Secure container gamma
         "59db794186f77448bc595262": 1, // 1x Secure Container epsilon
         "5fc64ea372b0dd78d51159dc": 4 // 5x cultis knifes
      },
      "5c093ca986f7740a1867ab12": { // Secure container kappa
         "5857a8bc2459772bad15db29": 1, // 1x Secure Container gamma
         "6656560053eaaa7a23349c86": 5 // 5x lega
      }
   }
}
```

## Build

Prerequisites:

- .NET 9+ SDK

From this folder:

```powershell
dotnet restore
dotnet build -c Release
```

The project is configured to output into:

```text
build\<client/server>\...
```

## Install

Copy the `server\HardmodeChallenge` folder to `SPT\user\mods` and the `client\HardmodeChallenge` to `bepinex\plugins`.

If you use **Headless** clients, they will need the client-side plugin as well, or extration limitations will not apply.