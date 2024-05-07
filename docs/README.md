# Nightshade: in-game color reshader

image here

This mod adds a highly configurable pixel shader to the game, allowing you to
adjust saturation, lightness, contrast, and color balance live during gameplay.
It is intended to serve as an alternative to [ReShade](https://reshade.me/),
for those players who can't (or would prefer not to) use that program. I doubt
it has as many features as ReShade, but since it runs within the game, it can
do a few extra things, like apply independently to the world and UI layers, and
automatically apply different profiles depending on the in-game season.

It also includes a depth-of-field ("tilt-shift") shader, for fun. That one is
not as configurable, though.

This mod is named after [the family of flowering
plants](https://en.wikipedia.org/wiki/Solanaceae).

## Special Thanks

A very big thank you goes out to my lovely beta testers, who agreed to try out
this mod before it was ready for general release, and who helped me find bugs
and improve the mod:

- burntcheese.
- .cozy.with.zoe
- ellipzist
- .huntersouls
- hylianprincess
- jessicanekos
- .logophile
- pokkky
- riotgirl5989
- shalassa

## Installation

Nightshade is just a SMAPI mod, so you install it like any other: unzip the mod
into your Mods folder and start playing.

Of course, you will need to have SMAPI 4.0+ and Stardew Valley 1.6+.

## Configuration

Nightshade hooks into [Generic Mod Config
Menu](https://www.nexusmods.com/stardewvalley/mods/5098) if you have it
installed. However, that menu only allows you to configure the keybinding
which will open Nightshade's custom config menu (implementing this menu was
required in order to 1. avoid blocking too much of the screen, and 2. allow
values to affect the game's render in real time).

The default keybinding is H. Press it during gameplay to open the menu:

image here

There are a lot of controls. Here is an explanation of how they work.

* **Colorize World/Colorize UI** \
    You can independently toggle whether the color settings apply to the world
    layer and the UI layer.
* **Colorize By Season** \
    Nightshade saves up to four color profiles in its config.json. If Colorize
    By Season is enabled, the profiles will be mapped in order to Spring,
    Summer, Fall, and Winter (the tab titles will change to reflect this). In
    the appropriate season, that profile will be applied automatically.
* **Profile Switcher** \
    Switch between color profiles. Whichever one you select will be rendered
    live, even if Colorize By Season is on and it doesn't match the current
    season. If Colorize By Season is disabled, the selected profile will be
    saved (see Save) as the active profile and will apply at all times.
* **Saturation/Lightness/Contrast** \
    Adjust the three
    Adjust saturation of the game's colors. Range: -100 to 100.
* **Lightness** \
    Adjust saturation of the game's colors. Range: -80 to 80.


## Compatibility

This mod should be compatible with almost everything, since all it does is
alter the rendering logic to add the shaders, which is an uncommon technique.
This means it should work with all existing recolors and retextures, all mods
that render conventionally (Fashion Sense, Alternative Textures, etc.), and all
mod-added assets. However, any mods that similarly mess with the game's render
cycle or render in nonstandard ways are probably not compatible.

I don't have a list of such mods, so please let me know if you find an
incompatible one.


## Roadmap

Features planned for (near) future updates:

* Allow different color profiles to apply to the world and UI layers.
* Allow loading of color presets from separate json files in a subdirectory.
* Bundle some presets with the mod.
* Patch some game functions to move


## Performance

This mod adds two pixel shaders, and to employ them it adds up to seven
full-screen draw calls per frame, four of which run the shaders (the remaining
three are just blits, to get pixels to another texture). These draws and
shaders have a cost, so if your computer is using an integrated or weak GPU,
you may drop frames with everything enabled.

In this case, your best bet is to disable shaders you can live without: turning
off either colorizer layer saves one shader draw and one blit, and turning off
depth of field saves two shader draws. The depth of field shader is more costly
to run than the colorizer, so I recommend disabling that first if your
performance suffers.
