

# Texture Overlayer



This plugin is designed to essentially replace baking texture mods in either Penumbra or GIMP

You'll use a Penumbra Texture Editor-esque UI to define texture layers which get combined to make your final product.
Layers are enabled and disabled based upon the current mod status and setting.
You can configure multiple textures to replace, the primary use-case being skin textures but can be used for clothing textures as well.

This project wouldn't be possible without these projects, they have my greatest thanks.

![GitHub Release](https://img.shields.io/github/v/release/xivdev/Penumbra?style=plastic&label=Penumbra) ![GitHub Release](https://img.shields.io/github/v/release/0ceal0t/Dalamud-VFXEditor?style=plastic&label=VfxEdit)



## To do

* Texture combining and caching
  * Using base56 encoded filenames to store in a folder in the users Penumbra dir
  * Adding the physical combination of textures
* Jsonify the individual texture set configs
  * Need to store all the pre-generated files names in a dictionary utilizing a bitmask as the key
  * Have to store the penumbra settings that the texture uses to cross-reference on a on mod setting change listener
* Implement a window to sit between choosing a texture and the main list of texture sets
  * Needs to have all the setting each individual layer needs to have
* Texture selection window cleanup
  * Implement the VFXEditor selection method of selecting mods so I can show users what settings are associated with a texture
* Setup the temporary mod framework



## Insanely rough mockups
![ffxiv_dx11_FzrCMuT9Pc](https://github.com/user-attachments/assets/e7894280-8ba9-4251-b096-d13eeeb27cdd)
![ffxiv_dx11_0igu2dP7Yw](https://github.com/user-attachments/assets/1083e658-fc07-4b44-8835-52c38f0c4b00)
