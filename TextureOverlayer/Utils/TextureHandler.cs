

using System;
using TextureOverlayer.Textures;

namespace TextureOverlayer.Utils;


public static class TextureHandler
{

    public static nint GetImGuiHandle(String file)
    {
        BaseImage img = Service.TextureManager.LoadTex(file);
        var _wrapped = Service.TextureManager.LoadTextureWrap(img);
        return _wrapped.ImGuiHandle;
    }
}
