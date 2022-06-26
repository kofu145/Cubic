using System;
using Silk.NET.OpenGL;
using static Cubic.Graphics.Platforms.OpenGL33.OpenGL33GraphicsDevice;

namespace Cubic.Graphics.Platforms.OpenGL33;

public class OpenGL33Framebuffer : Framebuffer
{
    public uint Handle;
    
    public override void AttachTexture(Texture texture, int colorAttachment = 0)
    {
        OpenGL33Texture tex = (OpenGL33Texture) texture;
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
    }
    
    internal OpenGL33Framebuffer(uint handle)
    {
        Handle = handle;
    }

    public override void Dispose()
    {
        Gl.DeleteFramebuffer(Handle);
    }
}