using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace TextureOverlayer;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsConfigWindowMovable { get; set; } = true;
    public bool SomePropertyToBeSavedAndWithADefault { get; set; } = true;
    public string ModRootDirectory { get; set; } = string.Empty;
    public string PluginFolder { get; set; } = string.Empty;

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Service.PluginInterface.SavePluginConfig(this);
    }
}
