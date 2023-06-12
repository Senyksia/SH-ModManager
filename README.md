<div align="center">

# SpiderHeck Mod Manager

:toolbox::spider::wrench:&nbsp;&nbsp;&nbsp;&nbsp;:computer::gear:
<br />**In-game plugin management and settings**
<br />A [BepInEx](https://github.com/BepInEx/BepInEx) plugin for the game [SpiderHeck](https://store.steampowered.com/app/1329500/SpiderHeck)

### [Features](#features) • [Installation](#installation) • [Developing](#developing) • [Contributing](#contributing) • [Changelog](#changelog)

</div>


## Features

- **Drop-in:**&nbsp;&nbsp;drag the .dll into your plugins folder, and that's it!
- **Automatic mod detection:**&nbsp;&nbsp;enable/disable any installed mod, without needing setup from the developer
- **Settings API:**&nbsp;&nbsp;mod developers can register for their own options menu
- **In-game UI**:&nbsp;&nbsp;change settings via the in-game UI, without having to search through `every.single.config.file.cfg`
- [ ] **Online support:**&nbsp;&nbsp;(Planned!)


## Installation

1. **Locate your SpiderHeck install directory**

   | `Steam library` -> `Right click SpiderHeck` -> `Manage` -> `Browse local files`. |
   |:--------------------------------------------------------------------------------:|
   | ![A visual guide to locating the SpiderHeck directory](../assets/locating_directory.png?raw=true) |

   On Windows, it will typically be located at `C:\Program Files (x86)\Steam\steamapps\common\SpiderHeck`.

2. **Install BepInEx 6**

   - Download the pre-release for BepInEx 6 ([x64](https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.1/BepInEx_UnityMono_x64_6.0.0-pre.1.zip) or [x86](https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.1/BepInEx_UnityMono_x86_6.0.0-pre.1.zip)).
   - Extract the contents into your SpiderHeck directory.

3. **Install ModManager**

   - Download the [latest release](https://github.com/Senyksia/SH-ModManager/releases/latest/download/ModManager.zip).
   - Extract the contents into your SpiderHeck directory.


## Developing

### Prerequisites

1. **Add a reference to ModManager.dll**

2. **Specify ModManager as a dependency**

### Example

Registering for an options menu is as simple as
```cs
new OptionsMenu("My Mod!");
```
From there, you can add whichever options you need!
```cs
OptionsMenu optionsMenu = new OptionsMenu("My Mod!");

optionsMenu.AddToggle("A toggle", Config.Bind<bool>("Main", "aToggle", true))
optionsMenu.AddToggle("Another toggle", Config.Bind<bool>("Main", "anotherToggle", false))
```
As you can see, adding an option element requires a `ConfigEntry`. This is what will hold the current value of the option - ModManager just handles binding it to a new UI element. For more information on `ConfigEntry` and BepInEx configuration files, see [this article](https://docs.bepinex.dev/v6.0.0-pre.1/articles/dev_guide/plugin_tutorial/4_configuration.html).

A realistic example might look like this:
```cs
using BepInEx;
using BepInEx.Configuration;
using ModManager.UI;

namespace HugeSpiders
{
    [BepInDependency("senyksia.spiderheck.modmanager")]
    [BepInPlugin("senyksia.spiderheck.hugespiders", "HugeSpiders", "1.0.0")]
    [BepInProcess("SpiderHeckApp.exe")]
    internal class HugeSpiders : BaseUnityPlugin
    {
        public readonly ConfigEntry<float> sizeMultiplier;
        public readonly ConfigEntry<bool> doCollision;

        private HugeSpiders() : base()
        {
            sizeMultiplier = Config.Bind("Main", "SizeMultiplier", 2f, "How much to multiply the spider's radius by.");
            doCollision = Config.Bind("Main", "DoCollision", true, "Should the spider's collision box be increased too?");

            OptionsMenu optionsMenu = new OptionsMenu("Huge Spiders");
            //optionsMenu.AddInputField("Size multiplier", sizeMultiplier); still need to add lol
            optionsMenu.AddToggle("Increase spider collision", doCollision);
        }

        private void Awake()
        {
            Logger.LogInfo($"Increasing spider size by {sizeMultiplier.Value}x!");
        }
    }
}
```


## Contributing

1. **Clone the latest source code**

   `git clone https://github.com/Senyksia/SH-ModManager.git`

2. **Link to your SpiderHeck directory**

   Create a file called `ModManager.csproj.user` next to `ModManager.csproj`.
   ```xml
   <Project>
     <PropertyGroup>
       <GameFolder>path\to\SpiderHeck</GameFolder>                                              <!-- User-defined absolute path to SpiderHeck -->
       <ReferencePath>$(ReferencePath);$(GameFolder)\SpiderHeckApp_Data\Managed</ReferencePath> <!-- Path to the SH game assemblies -->
       <AssemblySearchPaths>$(AssemblySearchPaths);$(ReferencePath)</AssemblySearchPaths>       <!-- Add our path to the search list -->
     </PropertyGroup>
   </Project>
   ```
   Replace `path\to\SpiderHeck` with the absolute path to your SpiderHeck directory. E.g.
   ```xml
   <GameFolder>C:\Program Files (x86)\Steam\steamapps\common\SpiderHeck</GameFolder>
   ```

3. **Compile**

   Using Visual Studio, a copy of the compiled .dll should be placed directly in your mod folder.
   > **Note**
   > The game needs to be closed while compiling.


## Changelog

See [CHANGELOG.md](https://github.com/Senyksia/SH-ModManager/blob/main/CHANGELOG.md) for version changes.
