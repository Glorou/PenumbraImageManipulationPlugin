using System.Collections.Generic;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using OtterGui.Log;
using Penumbra.Api.Api;
using Penumbra.Api.IpcSubscribers;
using TextureOverlayer.Interop;
using TextureOverlayer.Textures;
using TextureOverlayer.Windows;


namespace TextureOverlayer;

public class Plugin : IDalamudPlugin
{

    
    private const string CommandName = "/pmycommand";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("TextureOverlayer");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    private ModSelector ModSelector { get; init; }

    

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Service.penumbraApi = new PenumbraIpc(pluginInterface);
        // you might normally want to embed resources and load them from the manifest stream
        Service.penumbraApi.Modlist = new GetModList(pluginInterface).Invoke();
        
        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);
        ModSelector = new ModSelector(this);
        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(ModSelector);
        
        Service.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        pluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        pluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        pluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        Service.TextureManager = new TextureManager(Service.DataManager, new Logger(), Service.TextureProvider, pluginInterface.UiBuilder);
        
        // Add a simple message to the log with level set to information
        // Use /xllog to open the log window in-game
        // Example Output: 00:57:54.959 | INF | [TextureOverlayer] ===A cool log message from Sample Plugin===
        Service.Log.Information($"===A cool log message from {pluginInterface.Manifest.Name}===");
        
        



    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        Service.CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
    public void ToggleModUI() => ModSelector.Toggle();
}
