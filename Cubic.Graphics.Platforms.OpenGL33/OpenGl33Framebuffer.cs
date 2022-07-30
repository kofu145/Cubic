using System;
using Silk.NET.OpenGL;
using static Cubic.Graphics.Platforms.OpenGL33.OpenGl33GraphicsDevice;

namespace Cubic.Graphics.Platforms.OpenGL33;

public class OpenGl33Framebuffer : Framebuffer
{
    public uint Handle;

    public override bool IsDisposed { get; protected set; }

    private bool _hasColorAttachment;
    private DrawBufferMode _dbMode;
    private ReadBufferMode _rdMode;

    public override void AttachTexture(Texture texture, int colorAttachment = 0)
    {
        OpenGl33Texture tex = (OpenGl33Texture) texture;
        switch (tex.TextureUsage)
        {
            case TextureUsage.Texture:
                throw new GraphicsException("Regular Textures cannot be used as a framebuffer attachment.");
            case TextureUsage.Framebuffer:
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);
                switch (tex.Format)
                {
                    case PixelFormat.Depth24Stencil8:
                        Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                            FramebufferAttachment.DepthStencilAttachment, TextureTarget.Texture2D, tex.Handle, 0);
                        break;
                    case PixelFormat.DepthOnly:
                        Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                            FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, tex.Handle, 0);
                        break;
                    default:
                        Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                            (FramebufferAttachment) (int) FramebufferAttachment.ColorAttachment0 + colorAttachment,
                            TextureTarget.Texture2D, tex.Handle, 0);
                        _hasColorAttachment = true;
                        break;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (_hasColorAttachment)
        {
            Gl.DrawBuffer(_dbMode);
            Gl.ReadBuffer(_rdMode);
        }
        else
        {
            Gl.DrawBuffer(DrawBufferMode.None);
            Gl.ReadBuffer(ReadBufferMode.None);
        }
        
        GLEnum status = Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        if (status != GLEnum.FramebufferComplete)
            throw new Exception($"Framebuffer status: {status}");
    }
    
    internal OpenGl33Framebuffer(uint handle)
    {
        Handle = handle;

        Gl.GetInteger(GetPName.DrawBuffer, out int drawBuffer);
        Gl.GetInteger(GetPName.ReadBuffer, out int readBuffer);
        _dbMode = (DrawBufferMode) drawBuffer;
        _rdMode = (ReadBufferMode) readBuffer;
    }

    public override void Dispose()
    {
        if (IsDisposed) return;
        IsDisposed = true;
        Gl.DeleteFramebuffer(Handle);
    }
}