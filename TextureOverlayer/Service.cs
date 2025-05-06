using Dalamud.Interface.ImGuiFileDialog;
using TextureOverlayer.Interop;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.Resource;

namespace TextureOverlayer
{
    internal class Service
    {


        [PluginService]
        internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

        [PluginService]
        internal static ITextureProvider TextureProvider { get; private set; } = null!;

        [PluginService]
        internal static ICommandManager CommandManager { get; private set; } = null!;

        [PluginService]
        internal static IClientState ClientState { get; private set; } = null!;

        [PluginService]
        internal static IDataManager DataManager { get; private set; } = null!;

        [PluginService]
        internal static IFramework Framework { get; private set; } = null!;
        [PluginService]
        internal static IPluginLog Log { get; private set; } = null!;

        internal static Plugin Plugin { get; set; } = null!;
        public static PenumbraIpc penumbraApi { get; set; } = null!;
        public static Textures.TextureManager TextureManager { get; set; } = null!;
        public static FileDialogManager FileDialogManager { get; set; } = null!;
        public static Utils.DataService DataService { get; set; } = null!;

    }
}
