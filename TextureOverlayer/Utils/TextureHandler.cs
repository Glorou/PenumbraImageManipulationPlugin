

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Havok.Common.Serialize.Util;
using ImGuiNET;
using Newtonsoft.Json;
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


/// <summary> Class that encapsulates all data needed to create the combination</summary>
/// <remarks></remarks>
[Serializable]
public class ImageCombination
{
    public String Name {get; set;}
    String gamepath = string.Empty;
    bool enabled  = false;
    String fileName = string.Empty;
    int position = -1;
    [JsonIgnore]
    CombinedTexture comboTex = new CombinedTexture(new Texture(), new Texture());
    public int LoadState { get; set; } //0 == not loaded 1 == loading 2 == loaded
    List<ImageLayer> layers = new List<ImageLayer>();
    [JsonIgnore]
    public List<CombinedTexture> _sandwich = new List<CombinedTexture>();
    private (int width, int height) res;


    public ImageCombination(String displayName)
    {
        this.Name = displayName;
    }
    public String GamePath => gamepath;
    public bool Enabled => enabled;
    public String FileName => fileName;
    
    
    public bool PathExists(string path)
    {
        foreach (var texture in layers)
        {
            if (texture.GetTexture().Path.Equals(path))
            {
                return true;
            }
        }
        return false;
            
    }
    public List<ImageLayer> Layers => layers;
    
    
    
    public void AddLayer(string path)
    {
        Texture temp = new Texture();
        temp.Load(Service.TextureManager, path);
        layers.Add(new ImageLayer(temp, CombineOp.Over, ResizeOp.ToRight));
        Service.Framework.Run(Compile);
    }



    public async Task Compile()
    {
        _sandwich.Clear();
        LoadState = 1;


        {
            if (layers.Count == 1)
            {
                comboTex.Dispose();
                comboTex = new CombinedTexture(layers[0].GetTexture(), new Texture());
                res = layers[0].GetTexture().BaseImage.Dimensions;

            }
            else if (layers.Count >= 2)
            {
                _sandwich.Add(new CombinedTexture(layers[0].GetTexture(), new Texture()));
                _sandwich[0].Update();
                for (var i = 1; i < layers.Count; ++i)
                {
                    _sandwich[i - 1].GetCurrent().TextureWrap ??=
                        Service.TextureManager.LoadTextureWrap(_sandwich[i - 1].GetCurrent().RgbaPixels, res.width, res.height);
                    _sandwich.Add(new CombinedTexture(_sandwich[i - 1].GetCurrent(), layers[i].GetTexture()));
                    await newStackOps(_sandwich[^1]);
                }

                comboTex = _sandwich[^1];
            }

            LoadState = 2;
        }
    }



    public CombinedTexture CombinedTexture => comboTex;

    public async Task newStackOps(CombinedTexture tex)
    {
        tex.Update();
        tex.setOps();
        var waiting = tex.CombineImage();
        return;
    }

    
}




[Serializable]
public class ImageLayer
{

    [JsonIgnore]
    private Texture? _texture;

    private Matrix4x4 _multiplier  = Matrix4x4.Identity;
    private Vector4   _constant    = Vector4.Zero;
    private int       _offsetX;
    private int       _offsetY;
    private CombineOp _combineOp    = CombineOp.Over;
    private ResizeOp  _resizeOp     = ResizeOp.None;
    public Channels  _copyChannels = Channels.Red | Channels.Green | Channels.Blue | Channels.Alpha;
    private String path = string.Empty;
    
    
    public ImageLayer(Texture texture, CombineOp combineOp, ResizeOp resizeOp)
    {
        path = texture.Path;
        this._texture = texture;
        this._combineOp = combineOp;
        this._resizeOp = resizeOp;
    }
    

    public Texture GetTexture() => _texture;
    
    //TODO: make a file fetch function that will check that the file exists, if not requery to check if it still exists at all
} 



public static class TextureHandler
{

   /* public static nint GetImGuiHandle(ImageCombination file)
        => GetImGuiHandle(file.FileName);*/




/*    public static nint GetImGuiHandle(ImageCombination combo)  //need to add some cache shit here
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
    }*/
    

    
}
