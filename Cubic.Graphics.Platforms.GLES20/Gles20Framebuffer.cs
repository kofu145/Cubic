using System;
using Silk.NET.OpenGLES;
using static Cubic.Graphics.Platforms.GLES20.Gles20GraphicsDevice;

namespace Cubic.Graphics.Platforms.GLES20;

public class Gles20Framebuffer : Framebuffer
{
    public uint Handle;

    public override bool IsDisposed { get; protected set; }

    public override void AttachTexture(Texture texture, int colorAttachment = 0)
    {
        Gles20Texture tex = (Gles20Texture) texture;
        switch (tex.TextureUsage)
        {
            case TextureUsage.Texture:
                throw new GraphicsException("Regular Textures cannot be used as a framebuffer attachment.");
            case TextureUsage.Framebuffer:
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);
                Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                    (FramebufferAttachment) (int) FramebufferAttachment.ColorAttachment0 + colorAttachment,
                    TextureTarget.Texture2D, tex.Handle, 0);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        // TODO: Drawbuffer(?) and readbuffer here.
    }
    
    internal Gles20Framebuffer(uint handle)
    {
        Handle = handle;
    }

    public override void Dispose()
    {
        if (IsDisposed) return;
        IsDisposed = true;
        Gl.DeleteFramebuffer(Handle);
    }
}