

# Penumbra Image Manipulation Plugin



Penumbra Image Manipulation Plugin or pimp for short, is designed to replace the need for baking texture mods in either Penumbra or GIMP/Photoshop

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

![ffxiv_dx11_RX8Y7fEiB4](https://github.com/user-attachments/assets/80264d50-8d90-4c03-9473-68ec4403d894)
