

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FFXIVClientStructs;
using TextureOverlayer.Textures;

namespace TextureOverlayer.Utils;



public class ImageCombination
{
    private String displayName  = "Temp name";
    String gamepath = string.Empty;
    bool enabled  = false;
    String fileName = string.Empty;
    int position = -1;
    SortedList<int, ImageLayer> layers = new SortedList<int, ImageLayer>();

    public Dictionary<byte, string> lookuptable = new Dictionary<byte, string>();
    public ImageCombination(String displayName)
    {
        this.displayName = displayName;
    }
    public String Name => displayName;
    public String GamePath => gamepath;
    public bool Enabled => enabled;
    public String FileName => fileName;
    public int Position => position;
    



    public SortedList<int, ImageLayer> Layers
    {
        get { return layers; }
    }
    

    
    
    public void addLayer(ImageLayer layer)
    {
        layers.Add(layers.Count,layer);
    }
    
}


public class ImageLayer
{
    int priority { get; set; } = -1;
    bool enabled { get; set; } = false;
    bool baseLayer { get; set; } = false;
    private string modName { get; set; } = string.Empty;
    String filePath { get; set; }= string.Empty;  //for now we're only going to support Overlay, and if it's not the same size as the base layer, we're going to resize it to that
    private List<String> associtatedSettings { get; set; } = new List<string>();
    public String ModName()=> modName;
    public ImageLayer(String modName, String filePath, List<String> associtatedSettings)
    {
        this.modName = modName;
        this.filePath = filePath;
        this.associtatedSettings = associtatedSettings;
        
    }
    public String FilePath => filePath;
    
    //TODO: make a file fetch function that will check that the file exists, if not requery to check if it still exists at all
} 



public static class TextureHandler
{
    private static String path = string.Empty;
    private static BaseImage _baseImage;


    public static nint GetImGuiHandle(ImageCombination file)
        => GetImGuiHandle(file.FileName);
    public static nint GetImGuiHandle(ImageLayer file)
        => GetImGuiHandle(file.FilePath);

    public static nint GetImGuiHandle(String file)
    {
        try
        {
            if (file != path)
            {
                _baseImage = Service.TextureManager.LoadTex(file);
            }

            var _wrapped = Service.TextureManager.LoadTextureWrap(_baseImage);
            return _wrapped.ImGuiHandle;
        }
        catch (Exception e)
        {
            Service.Log.Error(e.ToString());
        }

        throw new InvalidOperationException();
    }


    public static String GetTextureUsage(String file)
    {
        return string.Empty;
        ;
    }
    
}
