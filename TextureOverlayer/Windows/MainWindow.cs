using System;
using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Lumina.Excel.Sheets;
using TextureOverlayer.Utils;

namespace TextureOverlayer.Windows;

public class MainWindow : Window, IDisposable
{

    private Plugin Plugin;

    private ImageCombination selectedCombination = null;
    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin)
        : base("GIMP but better##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };


        Plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        // Do not use .Text() or any other formatted function like TextWrapped(), or SetTooltip().
        // These expect formatting parameter if any part of the text contains a "%", which we can't
        // provide through our bindings, leading to a Crash to Desktop.
        // Replacements can be found in the ImGuiHelpers Class

        /*if (ImGui.Button("Select A mod"))
        {
            Plugin.ToggleModUI();
        }*/
        if (ImGui.Button("Create new Combined Texture"))
        {
            ImGui.OpenPopup("New Combined Texture##Window");
            
        }

        if (ImGui.BeginPopup("New Combined Texture##Window"))
        {
            var name = string.Empty;
            ImGui.TextUnformatted("Texture nickname:");
            ImGui.InputText("Nickname", ref name, 255);
            if (ImGui.Button("Confirm") && name != string.Empty)
            {
                Service.DataService.AddImageCombination(name);
                ImGui.CloseCurrentPopup();
            }
            ImGui.EndPopup();
        }

        if (ImGui.Button("OpenModUI"))
        {
            Plugin.ToggleModUI();
            
        }
        ImGui.Spacing();

        ImGui.TextUnformatted($"Combined Textures:");
        // Normally a BeginChild() would have to be followed by an unconditional EndChild(),
        // ImRaii takes care of this after the scope ends.
        // This works for all ImGui functions that require specific handling, examples are BeginTable() or Indent().
        using (var child = ImRaii.Child("SomeChildWithAScrollbar", Vector2.Zero, true))
        {
            // Check if this child is drawing
            if (child.Success)
            {

                foreach (var combo in Service.DataService.GetImageList())
                {

                        if(ImGui.Button(combo.Name))
                        {
                            
                        }
                }
            }
        }
        ImGui.SameLine();
        using (var previewer =ImRaii.Child("previewer"))
        {
            
            if (selectedCombination != null)
            {
                if
                //ImGui.Image(TextureHandler.GetImGuiHandle(filePreview), new Vector2((ImGui.GetContentRegionAvail().X ), (ImGui.GetContentRegionAvail().X )));
                //if (ImGui.Button($"Confirm Texture##{filePreview}"))
                {
                    
                }
                
            }

        }
    }
}
    
