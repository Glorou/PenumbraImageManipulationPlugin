using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using Newtonsoft.Json;

namespace TextureOverlayer.Utils;



public class DataService
{
    private List<ImageCombination> _allCombinations = new List<ImageCombination>();

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
            _allCombinations.RemoveAll(x => x.Name == name);
            return true;
        }catch (Exception e)
        {
            Service.Log.Error("Uh oh fucky wucky!!");
            return false;
        }
        
    }

            public  void LoadSelection( String item, out SelectedPenumbraMod loaded ) {
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
        => _allCombinations;


}
