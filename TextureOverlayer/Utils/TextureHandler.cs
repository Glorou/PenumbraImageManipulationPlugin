

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Numerics;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs;
using ImGuiNET;
using TextureOverlayer.Textures;

namespace TextureOverlayer.Utils;

public enum CombineOp
{
    LeftMultiply  = -4,
    LeftCopy      = -3,
    RightCopy     = -2,
    Invalid       = -1,
    Over          = 0,
    Under         = 1,
    RightMultiply = 2,
    CopyChannels  = 3,
}

public enum ResizeOp
{
    LeftOnly  = -2,
    RightOnly = -1,
    None      = 0,
    ToLeft    = 1,
    ToRight   = 2,
}

[Flags]
public enum Channels : byte
{
    Red   = 1,
    Green = 2,
    Blue  = 4,
    Alpha = 8,
}


public class ImageCombination
{
    private String displayName  = "Temp name";
    String gamepath = string.Empty;
    bool enabled  = false;
    String fileName = string.Empty;
    int position = -1;
     Texture comboTex = new Texture();
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
        => layers;
    
    public void addLayer(ImageLayer layer)
    {
        layers.Add(layers.Count,layer);
    }
    
    public Texture ComboTex
    {
        get;
        set;
    }

    public ImageLayer getLayerOrEmpty(String name)
    {
        foreach (ImageLayer layer in layers.Values)
        {
            if (layer.FilePath == name)
            {
                return layer;
            }
        }
        return null;
    }
    
}


public class ImageLayer
{
    bool fromPenumbra { get; set; } = false;
    private string modName { get; set; } = string.Empty;
  //for now we're only going to support Overlay, and if it's not the same size as the base layer, we're going to resize it to that
    private List<String> associatedSettings { get; set; } = new List<string>();
    public String ModName()=> modName;
    String filePath { get; set; }= string.Empty;
    private Texture tex = new Texture();
    
    private Matrix4x4 _multiplier  = Matrix4x4.Identity;
    private Vector4   _constant    = Vector4.Zero;
    private int       _offsetX;
    private int       _offsetY;
    private CombineOp _combineOp    = CombineOp.Over;
    private ResizeOp  _resizeOp     = ResizeOp.None;
    public Channels  _copyChannels = Channels.Red | Channels.Green | Channels.Blue | Channels.Alpha;
    
    public ImageLayer(String modName, String filePath, List<String> associatedSettings)
    {
        this.modName = modName;
        this.filePath = filePath;
        this.associatedSettings = associatedSettings;
        fromPenumbra = true;
        tex.Load(Service.TextureManager, this.filePath);
    }

    public ImageLayer(String filePath)
    {
        this.filePath = filePath;
        this.fromPenumbra = false;
        tex.Load(Service.TextureManager, this.filePath);
    }

    public Texture GetTexture() => tex;
    public String FilePath => filePath;
    
    //TODO: make a file fetch function that will check that the file exists, if not requery to check if it still exists at all
} 



public static class TextureHandler
{
    private static String path = string.Empty;
    private static BaseImage _baseImage;
    private static (BaseImage, TextureType) loadedFile;
    private static IDalamudTextureWrap _wrapped;
    private static BaseImage TexturePreview;
    private static CombinedTexture combinedTexture;
   /* public static nint GetImGuiHandle(ImageCombination file)
        => GetImGuiHandle(file.FileName);*/
    public static nint GetImGuiHandle(ImageLayer file)
        => GetImGuiHandle(file.FilePath);

    public static nint GetImGuiHandle(String file)
    {
        try
        {
            if (file != path)
            {
                loadedFile = Service.TextureManager.Load(file);
                _wrapped = Service.TextureManager.LoadTextureWrap(loadedFile.Item1);
            }


            return _wrapped.ImGuiHandle;
        }
        catch (Exception e)
        {
            Service.Log.Error(e.ToString());
        }

        throw new InvalidOperationException();
    }

    public static nint GetImGuiHandle(ImageCombination combo)  //need to add some cache shit here
    {
        try
        {
            LazyCombine(combo);

            return combinedTexture.GetCenter().TextureWrap.ImGuiHandle;
        }
        catch (Exception e)
        {
            Service.Log.Error(e.ToString());
        }
        
        return 0;
    }
    
    public static void LazyCombine(ImageCombination file)
    {

        var Base = file.Layers[0];
        if (file.Layers.Count == 2)
        {
            combinedTexture = new CombinedTexture(Base.GetTexture(), file.Layers[1].GetTexture());
            
        }
        if (file.Layers.Count > 2)
        {
            CombinedTexture temp = null;
            for (var i = 2; i < file.Layers.Count; i++)
            {
                temp = new CombinedTexture(combinedTexture.GetCenter(), file.Layers[i].GetTexture());
                combinedTexture = temp;
            }
        }
        file.ComboTex = combinedTexture.GetCenter();
        return;
    }
    public static String GetTextureUsage(String file)
    {
        return string.Empty;
        ;
    }
    
}
