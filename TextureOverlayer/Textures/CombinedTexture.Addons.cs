using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Interface.Utility;
using ImGuiNET;
using OtterGui;
using OtterGui.Raii;
using SixLabors.ImageSharp.PixelFormats;
using TextureOverlayer.Utils;

namespace TextureOverlayer.Textures;


//All the stuff to apply modifiers without using the native matrix code is going here 
public partial class CombinedTexture
{
    
    public Texture GetCurrent()
    {
        return _current;
    }

    public void setOps(ImageLayer layer)
    {

        _combineOp = (CombineOp)layer._combineOp;
        _resizeOp = ResizeOp.ToLeft;
    } 
    public bool SetMatrixOps(Matrix4x4 matrix)
    {
        _multiplierLeft = matrix;
        return true;
    }
    
    private void SubtractPixels(int y, ParallelLoopState _)
    {
        for (var x = 0; x < _leftPixels.Width; ++x)
        {
            var offset = (_leftPixels.Width * y + x) * 4;
            var left   = DataLeft(offset);
            var right  = DataRight(x, y);
            var alpha  = left.W;
            var rgba = alpha == 0
                           ? new Rgba32()
                           : new Rgba32(((left * left.W - right * right.W * (1 - right.W)) / alpha) with { W = alpha });
            _centerStorage.RgbaPixels[offset]     = rgba.R;
            _centerStorage.RgbaPixels[offset + 1] = rgba.G;
            _centerStorage.RgbaPixels[offset + 2] = rgba.B;
            _centerStorage.RgbaPixels[offset + 3] = rgba.A;
        }
    }
    
}
