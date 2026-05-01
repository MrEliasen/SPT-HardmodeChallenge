<p align="center">
  <img src="logo.png" alt="Logo" width="400">
</p>
Vagabond is a gameplay overhaul mod, which aims to turn Tarkov into an open world game.

You start with limited money to buy a simple loadout from Fence, with some meds if you are lucky, and your challenge is now to survive. Using only transits to move around Tarkov and specific extracts to get access to traders.

**Heavily inspired by Path To Tarkov, hopefully this mod can help scratch that itch.**

[![GitHub Issues or Pull Requests by label](https://img.shields.io/github/issues/MrEliasen/SPT-Vagabond/bug?style=for-the-badge&label=open%20Issues&color=red)
](https://github.com/MrEliasen/SPT-Vagabond/issues?q=is%3Aissue%20state%3Aopen%20label%3Abug)

#### Using SVM?
Disable the Raid Settings tab completely, it even being enabled is enough to cause conflicts, regardless of whether or not you changed anything within the tab. - Thank you [_liquidrage](https://forge.sp-tarkov.com/user/90917/liquidrage)!

#### Using ABPS ?
0.6.0 was tested with ABPS 2.0.16, and a graceful failover is added. If your version is not compatible, it will say so in the bepinex logs, but Vagabond with continue to work.

#### Does Quests Work
They should. If its a quest which requires specific extractions, those will be available only when you have the quest(s). Using such extraction will take you back to the quest giver. Do let me know if I missed any quests / if you find any quests you cannot complete.

## Main Features
- Place your hideout entrance anywhere (Press CTRL+P in raid to place your hideout entrance)
- Per-trader (and hideout if playing with friends) stash
- Use trader specific extractions to get access to their shop.
- Custom trader support (Reserve exfil to get access)
- Custom extractions and transits
- Remember last exit/transit location
- Reduce raid loot if you repeat the same raid
- Modding API

## Compatibility

Any mod which makes changes to Extractions, Transits or player spawning (Like selectable entry mod or interaction mods), will likely conflict with this mod and prevent extracts from working.    
Labyrinth has not been tested with this mod.. but.. should hopefully work.

## Install

1. Download the latest version
2. Extract and copy the `SPT\user\mods\Vagabond` folder to `SPT\user\mods` and the `BepInEx\plugins\Vagabond` to `BepInEx\plugins`.
3. If you use **Headless** clients, you will need to add the client plugin to the headless spt client as well.
4. Create a new profile, and it will get enrolled as a new Vagabond.

## Map
[View Full Size](https://raw.githubusercontent.com/MrEliasen/SPT-Vagabond/refs/heads/master/screenshots/game-map.webp)    
If you want to see the trader locations, [click here](https://github.com/MrEliasen/SPT-Vagabond/tree/master/screenshots/traders).
![SPT Vagabond Map](https://raw.githubusercontent.com/MrEliasen/SPT-Vagabond/refs/heads/master/screenshots/game-map.webp)

## Limits / Issues

See knows issues/limitations with each released version. Below are other general issues/limitations across all versions:

- SVM Extracts settings will cause conflicts, like preventing extractions etc.
- (Fika) If one of your teammates die and you use a transit after, the game will get stuck. I don't think this is a mod issue however.
- (Fika) the spawn-in location  and loot amount is determined by the player who initiates the game.
- Hideout exfils persist until server restart. Other players will be able to use them as extracts until then.

## Credit

[Trap](https://forge.sp-tarkov.com/user/15099/trap), for the original PTT mod, serving as strong inspiration.    
[Sacrificial Lamb](https://forge.sp-tarkov.com/user/108489/sacrificial-lamb), for testing a lot of cross compatibility between Vagabond, other mods and SVM settings on the initial release.    
[DanW](https://forge.sp-tarkov.com/user/27632/danw), for the Hardcore Rules mod which I nicked some patches from.    
[GhostFenixx](https://forge.sp-tarkov.com/user/3972/ghostfenixx), for the SVM mod which I also nicked some patches from.    
[acidphantasm](https://forge.sp-tarkov.com/user/48110/acidphantasm) for the item limits begone mod,  which I also nicked some code from.


## Config

Configuration is quite extensive, there are several config files, from extracts to transits, below is the "core" config file, the rest you will find in the same dir.

```json
{
  // if you die for whatever reason (according to the game), all stashes and equipment gets wiped.
  "ResetOnDeath": false,
  // The amount of money a player starts with
  "StartingRoubles": 175000,
  // makes fence less random and useful as a starter vendor - recommend removing other traders
  "EnableFenceChanges": true,
  "DisableFlea": true,
  // disables events such as halloween etc.
  "DisableEvents": true,
  // players can only send mail with attachments to eachother if:
  // "same-exit" - they share raid + exit
  // "same-map" - they share raid
  // "anywhere" - no limits
  "MailAttachmentLimit": "same-exit",
  // if enabled, you can choose what map you want to go to and won't be locked to your current location.
  "EnablePickRaidLocation": false,
  // first time you enter a raid, whatever is left over in the stash is wiped.
  "WipeStashOnFirstRaidEntry": true,
  // adjust how long raids last by this amount of minutes, negagive values supported by be careful.
  "AdjustRaidTimeMins": 60,
  // if enabled, you can continue to place the hideout without any limitation.
  // if disabled, to be able to relocate you will need to complete the "Fresh Foundations" quest from Skier (repeatable).
  "AllowHideoutRelocation": false,
  // Where you will be placed when you die, options are: 
  // "hideout" - will send you to your hideout if you have one, otherwise "stays".
  // "stay" - you stay at your last known map/transit
  // "custom" - goes to the raid and specified exfil
  // acceptable raid names are:
  //  FactoryDay,
  //  FactoryNight,
  //  GroundZero,
  //  Streets,
  //  Woods,
  //  Customs,
  //  Interchange,
  //  Lighthouse,
  //  Reserve,
  //  Shoreline,
  //  Labs,
  //  Labyrinth
  "OnDeathGoTo": "hideout",
  "OnDeathGoToRaid": "",
  "OnDeathGoToExfilIdentifier": "",
  // This is where new players start.
  // acceptable raid names are:
  //  FactoryDay,
  //  FactoryNight,
  //  GroundZero,
  //  Streets,
  //  Woods,
  //  Customs,
  //  Interchange,
  //  Lighthouse,
  //  Reserve,
  //  Shoreline,
  //  Labs,
  //  Labyrinth
  "StartRaid": "Streets",
  "StartExfilIdentifier": "VGB_EXT_FENCE",
  // will allow per-trader/per-extract (eg other players hideouts) virtual stashes.
  // if you disable this, your own permanent stash is used like normal.
  "EnableVirtualStashes": true,
  // if enabled, will wipe all virtual stashes when you enter a raid, making virtural
  // stashes entirely temporary. This will prevent "AllowPostRaidHealing" from working.
  "WipeVirtualStashesOnRaidEntry": false,
  // if enabled, all status effects (broken limbs, bleeds etc) will be healed when you die.
  "HealStatusEffectsOnDeath": true,
  // allow post-raid healing at Therapist (still requires money left at the Therapist)
  "AllowPostRaidHealing": true,
  // add Fence as an always available trader in hideout
  "AddFenceToHideout": false,
  // if enabled, you can use another player's hideout exit to access your own hideout
  "ShareHideoutExits": false,
  // how much it should cost to relocate the hideout
  "HideoutRelocationFee": 350000,
  // What loyalty level is required to accept the quest to get the trader to join your hideout?
  "JoinHideoutTherapistLoyaltyLevel": 2,
  "JoinHideoutJaegerLoyaltyLevel": 2,
  "JoinHideoutMechanicLoyaltyLevel": 2,
  "JoinHideoutPeacekeeperLoyaltyLevel": 2,
  "JoinHideoutPraporLoyaltyLevel": 2,
  "JoinHideoutRagmanLoyaltyLevel": 2,
  "JoinHideoutSkierLoyaltyLevel": 2,
  // limit mail from traders so you can only access mail attachments if the trader is available where you are.
  "LimitTraderMailAccess": true,
  // if enabled, each consecutive successful extract from the same map reduces the loot the next raid into that map.
  // This streak resets when you successfully extract from a different map. death/aborts do not change the streak.
  // factory day and night count as the same map.
  "EnableConsecutiveMapLootReduction": true,
  // Loot multiplier. 0.5 = halving each repeated raid (1st=100%, 2nd=50%, 3rd=25%, etc)
  "ConsecutiveMapLootReductionRate": 0.5,
  // the minimum amount of loot regardless of streeak. 0.05 = 5% of the normal amount of loot.
  "ConsecutiveMapLootReductionMin": 0.05,
  // if enabled, your limbs are healed to this amount when you die.
  // Examples:
  // 1.0 = 100%
  // 0.25 = 25%
  // 0.0 = game default.
  "HealthOnDeath": 0.0
}

```