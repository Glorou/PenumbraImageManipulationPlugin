using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace TextureOverlayer.Textures;

public unsafe class SafeTextureHandle : SafeHandle
{
    public FFXIVClientStructs.FFXIV.Client.Graphics.Kernel.Texture* Texture
        => (FFXIVClientStructs.FFXIV.Client.Graphics.Kernel.Texture*)handle;

    public override bool IsInvalid
        => handle == 0;

    public SafeTextureHandle(FFXIVClientStructs.FFXIV.Client.Graphics.Kernel.Texture* handle, bool incRef, bool ownsHandle = true)
        : base(0, ownsHandle)
    {
        if (incRef && !ownsHandle)
            throw new ArgumentException("Non-owning SafeTextureHandle with IncRef is unsupported");

        if (incRef && handle != null)
            handle->IncRef();
        SetHandle((nint)handle);
    }

    public void Exchange(ref nint ppTexture)
    {
        lock (this)
        {
            handle = Interlocked.Exchange(ref ppTexture, handle);
        }
    }

    public static SafeTextureHandle CreateInvalid()
        => new(null, false);

    protected override bool ReleaseHandle()
    {
        nint handle;
        lock (this)
        {
            handle      = this.handle;
            this.handle = 0;
        }

        if (handle != 0)
            ((FFXIVClientStructs.FFXIV.Client.Graphics.Kernel.Texture*)handle)->DecRef();

        return true;
    }
}
