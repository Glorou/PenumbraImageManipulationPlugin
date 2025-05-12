

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
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
    SubtractChannels = 4,
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

    private bool enabled;
    String fileName = string.Empty;
    int position = -1;
    private (int width, int height) res;
    List<ImageLayer> layers = new List<ImageLayer>();
    public (Guid, string) collection;
    public String _gamepath = string.Empty;
    
    
    [JsonIgnore]
    private List<CombinedTexture> _sandwich = new List<CombinedTexture>();
    [JsonIgnore]
    private CombinedTexture comboTex = new CombinedTexture(new Texture(), new Texture());
    [JsonIgnore]
    public int LoadState { get; set; } //0 == not loaded 1 == loading 2 == loaded


    

    public ImageCombination(String displayName)
    {
        this.Name = displayName;
        enabled = false;
    }
    public String FileName
    {
        get => fileName;
        set => fileName = value;
    }



    public (int width, int height) Res
    {
        get => res;
        set => res = value;
    }

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

    public bool Enabled
    {
        get => enabled;
        set
        {
            enabled = value;
            if (enabled)
            {
                Service.penumbraApi.AddTemporaryMod(this);
            }
            else
            {
                Service.penumbraApi.RemoveTemporaryMod(this);
            }
        }
    }
    
    public void AddLayer(string path)
    {
        layers.Add(new ImageLayer(path, CombineOp.Over, ResizeOp.ToRight));
        Service.Framework.Run(Compile);
    }

    public void RemoveLayer(ImageLayer layer)
    {
        layers.Remove(layer);
        Compile();
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
                    if (layers[i]._enabled)
                    {
                        _sandwich[^1].GetCurrent().TextureWrap ??=
                            Service.TextureManager.LoadTextureWrap(_sandwich[^1].GetCurrent().RgbaPixels, res.width, res.height);
                        _sandwich.Add(new CombinedTexture(_sandwich[^1].GetCurrent(), layers[i].GetTexture()));
                        
                        await newStackOps(_sandwich[^1], layers[i]);
                    }

                }

                comboTex = _sandwich[^1];
            }

            LoadState = 2;
        }
    }


    [JsonIgnore]
    public CombinedTexture CombinedTexture => comboTex;

    public async Task newStackOps(CombinedTexture tex, ImageLayer layer)
    {
        
        tex.Update();
        tex.setOps(layer);
        var waiting = tex.CombineImage();
        return;
    }

    
}




[Serializable]
public class ImageLayer
{



    public Matrix4x4 _multiplier  = Matrix4x4.Identity;
    public Vector4   _constant    = Vector4.Zero;
    public int       _offsetX;
    public int       _offsetY;
    public CombineOp _combineOp    = CombineOp.Over;
    public ResizeOp  _resizeOp     = ResizeOp.None;
    public Channels  _copyChannels = Channels.Red | Channels.Green | Channels.Blue | Channels.Alpha;
    public Blake3.Hash _fileHash;
    public bool _enabled = true;
    public String _friendlyName = string.Empty;
    
    [JsonIgnore]
    private Texture? _texture;

    public ImageLayer(String _path, CombineOp combineOp, ResizeOp resizeOp, bool enabled = true)
    {

        Texture texture = new Texture();
        _fileHash = Service.CacheService.TryGetCache(texture, _path);
        this._texture = texture;
        this._combineOp = combineOp;
        this._resizeOp = resizeOp;
        _enabled = enabled;
        _friendlyName = _path.Split('\\').Last().Split(".").First();
    }
    
    public void FlipState()
    {
        _enabled = !_enabled;
        
    }

    public String getEyecon()
    {
        return _enabled ? FontAwesomeIcon.Eye.ToIconString() : FontAwesomeIcon.EyeSlash.ToIconString();
    }
    
    public Texture GetTexture() => _texture;


    //TODO: make a file fetch function that will check that the file exists, if not requery to check if it still exists at all
} 



public static class TextureHandler
{
    public static readonly IReadOnlyList<string> ResizeOpLabels = new string[]
    {
        "Over",
        "Under",
        "Right Multiply",
        "Copy Channels",
        "Subtract Channels",
    };

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
