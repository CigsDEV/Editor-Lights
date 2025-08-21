# Baldi Level Editor – 💡 $\textcolor{#ff004d}{L}\textcolor{#ff8a00}{i}\textcolor{#ffd300}{g}\textcolor{#00e436}{h}\textcolor{#29adff}{t}\textcolor{#7a00ff}{s}\textcolor{#ff77a8}{!}$ 💡

Source for **BB+ Editor Lights v0.8.x**. Not actively maintained for a while, but PRs will get love when necessary.

## Dependencies

* **[Legacy Level Editor (0.11x)](https://gamebanana.com/wips/84160)**  
  Required. If you’re asking *why*, you probably haven’t opened the editor yet.

* **[MTM101 API](https://github.com/benjaminpants/MTM101BMDE)**  
  The core API this plugs into (primarily released on GameBanana).

* **[Level Loading System for BB+](https://gamebanana.com/mods/508477)**  
  Lets the editor add/remove objects without issue.

* **[BepInEx](https://github.com/BepInEx/BepInEx)**  
  The plugin framework. You need this or nothing loads.

## Overview

* $\textcolor{#ff004d}{Plugin.cs}$ - [source](https://github.com/CigsDEV/Editor-Lights/blob/main/EditorLights/Plugin.cs)  
  Bootstraps everything: loads sprites, builds editor prefabs, wires aliases. The front door operation essentially. 

* $\textcolor{#ff8a00}{FixLighting.cs}$ - [source](https://github.com/CigsDEV/Editor-Lights/blob/main/EditorLights/FixLighting.cs)  
  Smacks the lighting system on load so playmode isn’t pitch black.

* $\textcolor{#ffd300}{ColoredLight.cs}$ - [source](https://github.com/CigsDEV/Editor-Lights/blob/main/EditorLights/ColoredLight.cs)  
  The actual placeable light source. You drop it, it <img src="assets/glow.svg" alt="glows" height="22">. Simple.


* $\textcolor{#29adff}{ColorLookup.cs}$ - [source](https://github.com/CigsDEV/Editor-Lights/blob/main/EditorLights/ColorLookup.cs)  
  Fresh approach to parsing base + custom colors (HTML hex friendly).

* $\textcolor{#7a00ff}{Init.cs}$ - [source](https://github.com/CigsDEV/Editor-Lights/blob/main/EditorLights/EditorUtils/Init.cs)  
  Registers objects into the editor’s categories. If you’re adding tools, you’re looking for this.

* $\textcolor{#ff77a8}{objtool.cs}$ *(by bigthinker)* - [source](https://github.com/CigsDEV/Editor-Lights/blob/main/EditorLights/EditorUtils/objtool.cs)  
  Tiny helper that injects tools defined in `Init.cs`.

## Contributing
Issues and PRs welcome! Please don’t abuse or over-use this source code.  
Like, I don’t care if you add 999 hard-coded colors in your fork, but I probably won’t accept them if pushed to the main branch!
