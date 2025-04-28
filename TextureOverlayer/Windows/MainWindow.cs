using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface.ImGuiFileDialog;
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

        
        Service.FileDialogManager = new FileDialogManager();

        Plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        // Do not use .Text() or any other formatted function like TextWrapped(), or SetTooltip().
        // These expect formatting parameter if any part of the text contains a "%", which we can't
        // provide through our bindings, leading to a Crash to Desktop.
        // Replacements can be found in the ImGuiHelpers Class


        
        Service.FileDialogManager.Draw();




        ImGui.BeginGroup();
        if (ImGui.Button("OpenUI"))
        {
            Plugin.ToggleModUI();

        }

        ImGui.Spacing();

        ImGui.TextUnformatted($"Combined Textures:");
        // Normally a BeginChild() would have to be followed by an unconditional EndChild(),
        // ImRaii takes care of this after the scope ends.
        // This works for all ImGui functions that require specific handling, examples are BeginTable() or Indent().
        using (var child = ImRaii.Child("ComboPicker", new Vector2(ImGui.GetContentRegionAvail().X * 0.20f, ImGui.GetContentRegionAvail().Y * 0.80f), true))
        {
            // Check if this child is drawing
            if (child.Success)
            {

                foreach (var combo in Service.DataService.AllCombinations)
                {

                    if (ImGui.Selectable(combo.Name))
                    {
                        selectedCombination = combo;
                    }
                }
            }
        }
        if (ImGui.Button("Create new Combined Texture"))
        {
            ImGui.OpenPopup("New Combined Texture##Window");

        }
        using(var popup = ImRaii.Popup("New Combined Texture##Window"))
        {
            if (popup)
            {
                ImGui.TextUnformatted("Texture nickname:");
                ImGui.InputText("", ref name, 255);
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
        ImGui.EndGroup();
        

        ImGui.SameLine();
        using (var previewer = ImRaii.Child("previewer"))
        {

            if (selectedCombination != null && selectedCombination.FileName != string.Empty)
            {

                ImGui.Image(TextureHandler.GetImGuiHandle(selectedCombination),
                            new Vector2((ImGui.GetContentRegionAvail().X), (ImGui.GetContentRegionAvail().X)));
                if (ImGui.Button($"Confirm Texture##{selectedCombination.Name}")){}


            }/*else if(selectedLayer != null && selectedLayer.FilePath != string.Empty)
            {
                ImGui.Image(TextureHandler.GetImGuiHandle(selectedLayer),
                            new Vector2((ImGui.GetContentRegionAvail().X * .75f), (ImGui.GetContentRegionAvail().X * .75f)));
                
            }*/
            else
            {
                ImGui.Image(Service.TextureProvider.GetFromManifestResource(Assembly.GetExecutingAssembly(), "TextureOverlayer.Placeholder.png").GetWrapOrEmpty().ImGuiHandle,
                            new Vector2((ImGui.GetContentRegionAvail().X * .75f), (ImGui.GetContentRegionAvail().X * .75f)));
            }

            ImGui.SameLine();
            ImGui.BeginGroup();
            if (selectedCombination != null)
            {
                
                using(var child = ImRaii.Child("imagestack",
                                                new Vector2(ImGui.GetContentRegionAvail().X,
                                                            ImGui.GetContentRegionAvail().Y * 0.50f), true))
                {
                    // Check if this child is drawing
                    if (child.Success)
                    {
                        foreach (var image in selectedCombination.Layers)
                        {
                            if (ImGui.Selectable($"{image.Value.FilePath.Split('\\').Last()}"))
                            {
                                selectedLayer = image.Value;
                            }
                        }
                        
                    }
                }
                ImGui.TextUnformatted("New Layer from:");

                if(ImGui.Button("Penumbra mod"))
                {
                    Service.DataService.SetSelectedCombo(selectedCombination.Name);
                    Plugin.ToggleModUI();
                }
                ImGui.SameLine();
                if(ImGui.Button("File"))
                {
                    
                    Service.FileDialogManager.OpenFileDialog("Select Image","Image Files {.png, .tga, .dds, .bmp, .tex}",
                                                             (success, path) =>
                                                             {
                                                                 if (success)
                                                                 {
                                                                     selectedCombination.addLayer(new ImageLayer(path[0]));
                                                                 }
                                                                 
                                                             }, 1, null );
                }
                
                
            }
            
            ImGui.EndGroup();
            
        }
        //TODO: Fix this popup
        

    }
}    

    
