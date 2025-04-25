using System;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.Sheets;
using TextureOverlayer.Utils;

namespace TextureOverlayer.Windows;

public class MainWindow : Window, IDisposable
{


    private Plugin Plugin;
    private static string name = string.Empty;
    private ImageCombination selectedCombination = null;
    private ImageLayer selectedLayer = null;
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


        
        
        //TODO: Fix this popup
        if (ImGui.Button("Create new Combined Texture"))
        {
            ImGui.OpenPopup("New Combined Texture##Window");

        }

        using(var popup = ImRaii.Popup("New Combined Texture##Window"))
        {
            if (popup)
            {
                ImGui.TextUnformatted("Texture nickname:");
                ImGui.InputText("Nickname", ref name, 255);
                if (ImGui.Button("Confirm"))
                {
                    if (!name.IsNullOrWhitespace())
                    {
                        Service.DataService.AddImageCombination(name);
                        ImGui.CloseCurrentPopup();
                    }

                }
            }

        }

        
        if (ImGui.Button("OpenUI"))
        {
            Plugin.ToggleModUI();

        }

        ImGui.Spacing();

        ImGui.TextUnformatted($"Combined Textures:");
        // Normally a BeginChild() would have to be followed by an unconditional EndChild(),
        // ImRaii takes care of this after the scope ends.
        // This works for all ImGui functions that require specific handling, examples are BeginTable() or Indent().
        using (var child = ImRaii.Child("ComboPicker", new Vector2(ImGui.GetContentRegionAvail().X * 0.40f, ImGui.GetContentRegionAvail().Y * 0.80f), true))
        {
            // Check if this child is drawing
            if (child.Success)
            {

                foreach (var combo in Service.DataService.AllCombinations)
                {

                    if (ImGui.Button(combo.Name))
                    {
                        selectedCombination = combo;
                    }
                }
            }
        }

        ImGui.SameLine();
        using (var previewer = ImRaii.Child("previewer"))
        {

            if (selectedCombination != null && selectedCombination.FileName != string.Empty)
            {

                ImGui.Image(TextureHandler.GetImGuiHandle(selectedCombination),
                            new Vector2((ImGui.GetContentRegionAvail().X), (ImGui.GetContentRegionAvail().X)));
                if (ImGui.Button($"Confirm Texture##{selectedCombination.Name}")){}


            }else if(selectedLayer != null && selectedLayer.FilePath != string.Empty)
            {
                ImGui.Image(TextureHandler.GetImGuiHandle(selectedLayer),
                            new Vector2((ImGui.GetContentRegionAvail().X), (ImGui.GetContentRegionAvail().X)));
                
            }
            else
            {
                ImGui.Image(Service.TextureProvider.GetFromManifestResource(Assembly.GetExecutingAssembly(), "TextureOverlayer.Placeholder.png").GetWrapOrEmpty().ImGuiHandle,
                            new Vector2((ImGui.GetContentRegionAvail().X * .5f), (ImGui.GetContentRegionAvail().X * .5f)));
            }

            if (selectedCombination != null)
            {
                ImGui.SameLine();
                using(var child = ImRaii.Child("imagestack",
                                                new Vector2(ImGui.GetContentRegionAvail().X * 0.50f,
                                                            ImGui.GetContentRegionAvail().Y * 0.50f), true))
                {
                    // Check if this child is drawing
                    if (child.Success)
                    {
                        foreach (var image in selectedCombination.Layers)
                        {
                            if (ImGui.Selectable($"{image.Key}"))
                            {
                                selectedLayer = image.Value;
                            }
                        }
                        
                    }
                }
                if(ImGui.Button("Add new Layer from Penumbra"))
                {
                    Service.DataService.SetSelectedCombo(selectedCombination.Name);
                    Plugin.ToggleModUI();
                }
            }
            
        }

    }
}    

    
