using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using Dalamud.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OtterTex;
using TextureOverlayer.Textures;

namespace TextureOverlayer.Utils;



//TODO: Save the configs 
//TODO: Copy entire class when selected, so we dont have to compare to the file? maybe?
public class DataService
{
    private List<ImageCombination> _allCombinations = new List<ImageCombination>();

    public List<ImageCombination> AllCombinations => _allCombinations;
    private string selectedCombination= string.Empty;
    public bool AddImageCombination(string displayName )
    {
        try
        {
            _allCombinations.Add(new ImageCombination(displayName));
            return true;
        }
        catch (Exception e)
        {
            Service.Log.Error("Uh oh fucky wucky!!");
            return false;
        }
    }

    public ImageCombination GetImageCombination(String name)
    {
        return _allCombinations.Find(x => x.Name == name);
    }

    public bool RemoveImageCombination(String name)
    {
        try
        {
            var temp = _allCombinations.Find(x => x.Name == name);
            temp.CombinedTexture.Dispose();
            foreach(var layer in temp.Layers)
            {
                layer.GetTexture().Dispose();
            }
            _allCombinations.RemoveAll(x => x.Name == name);
            return true;
        }catch (Exception e)
        {
            Service.Log.Error("Uh oh fucky wucky!!");
            return false;
        }
        
    }


    public void SetSelectedCombo(string name)
    {
        selectedCombination = name;
    }

    public String GetSelectedCombo() => selectedCombination;

    public void ClearSelectedCombo() => selectedCombination = String.Empty;


    public void WriteConfig(ImageCombination combination)
    {
        var json = JsonConvert.SerializeObject(combination, Formatting.Indented);
        FilesystemUtil.WriteAllTextSafe(Service.Configuration.PluginFolder + $"\\{combination.Name}.json", json);
        
    }


    
    //TODO: Add support for BC compression
    public string WriteTexFile(ImageCombination combination)
    {
        Service.TextureManager.SaveAs(CombinedTexture.TextureSaveType.AsIs, false, true,
                                      combination.CombinedTexture.GetCurrent().BaseImage,
                                      Service.Configuration.PluginFolder + $"\\{combination.Name}.tex",
                                      combination.CombinedTexture.GetCurrent().RgbaPixels, combination.Res.width, combination.Res.height);
        return combination.Name + ".tex";
    }

    public ImageCombination ReadConfig(String path)
    {
        try
        {
            using StreamReader reader = new(path);
            string json = reader.ReadToEnd();
            JObject imageCombName = JObject.Parse(json);
            JToken comboName = imageCombName["Name"];
            var tempCombo = new ImageCombination(comboName.ToString());
            JsonConvert.PopulateObject(json,tempCombo);
            return tempCombo;
        }
        catch (Exception e)
        {

        }
        
        return null;
    }

    //TODO: Refactor for use here - only single mod at a time, dont bother caching for now
            //TODO: We only really need the file name, the mod its from (lazily match to accomodate helio fucked up names) and the settings associated
           /* public  void LoadSelection( String item, out SelectedPenumbraMod loaded ) {
            loaded = new();
            var files = new Dictionary<string, List<(string, string)>>();

            var baseModPath = Service.penumbraApi.GetModDirectory();
            if( string.IsNullOrEmpty( baseModPath ) ) return;

            try {
                var modPath = Path.Join( baseModPath, item );
                loaded.Meta = JsonConvert.DeserializeObject<PenumbraMeta>( File.ReadAllText( Path.Join( modPath, "meta.json" ) ) );

                var modFiles = Directory.GetFiles( modPath ).Where( x => x.EndsWith( ".json" ) && !x.EndsWith( "meta.json" ) );
                foreach( var modFile in modFiles ) {
                    try {
                        var modFileName = Path.GetFileName( modFile ).Replace( ".json", "" );
                        if( modFileName == "default_mod" ) {
                            var mod = JsonConvert.DeserializeObject<PenumbraModStruct>( File.ReadAllText( modFile ) );
                            if( mod.Files != null ) {
                                var defaultFiles = new List<(string, string)>();
                                AddToFiles( mod?.Files, defaultFiles, modPath );
                                files["default_mod"] = defaultFiles;
                            }
                        }
                        else {
                            var group = JsonConvert.DeserializeObject<PenumbraGroupStruct>( File.ReadAllText( modFile ) );
                            if( group.Options != null ) {
                                foreach( var option in group.Options.Where( x => x.Files != null ) ) {
                                    var optionFiles = new List<(string, string)>();
                                    AddToFiles( option?.Files, optionFiles, modPath );
                                    files[$"{group.Name} / {option.Name}"] = optionFiles;
                                }
                            }
                        }
                    }
                    catch( Exception e ) {
                        Dalamud.Error( e, modFile );
                    }
                }
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error reading Penumbra mods" );
            }

            loaded.SourceFiles = files.ToDictionary(
                x => x.Key,
                x => x.Value.Where( y => Path.Exists( y.Item2 ) ).Select( y => $"{y.Item1}|{y.Item2}" ).ToList() // actually use local path
            );

            loaded.ReplaceFiles = files.ToDictionary(
                x => x.Key,
                x => x.Value.Select( y => y.Item1 ).ToList()
            );
        }
    
    public List<ImageCombination> GetImageList()
        => _allCombinations;*/


}
