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

## Config

Configs live in `SPT/user/mods/Vagabond/config/`. Restart the SPT server after edits.

- [docs/CONFIG.md](docs/CONFIG.md) - core mod settings (`vagabond.json`)
- [docs/EXFILS.md](docs/EXFILS.md) - extracts + transits
- [docs/TRANSITIONS.md](docs/TRANSITIONS.md) - vanilla SPT transit landing spots
- [docs/TRADERS.md](docs/TRADERS.md) - which extract unlocks which trader
- [docs/KEYBINDINGS.md](docs/KEYBINDINGS.md) - modder dump hotkeys (F8/F9/F10) for authoring exfils/transits

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