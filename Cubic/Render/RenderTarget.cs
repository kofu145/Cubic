using System.Drawing;
using Cubic.Graphics;
using Silk.NET.OpenGL;
using Framebuffer = Cubic.Graphics.Framebuffer;
using PixelFormat = Silk.NET.OpenGL.PixelFormat;

namespace Cubic.Render;

public class RenderTarget : Texture
{
    internal Framebuffer Framebuffer;

    private Cubic.Graphics.Texture _depthTexture;

    public readonly RenderTargetFlags Flags;

    public RenderTarget(Size size, RenderTargetFlags flags = RenderTargetFlags.None, bool autoDispose = true) : base(autoDispose)
    {
        Flags = flags;
        CreateTarget(size, flags);
    }

    public void Resize(Size newSize)
    {
        Dispose();
        CreateTarget(newSize, Flags);
    }

    private void CreateTarget(Size size, RenderTargetFlags flags)
    {
        GraphicsDevice device = CubicGraphics.GraphicsDevice;

        Framebuffer = device.CreateFramebuffer();
        InternalTexture = device.CreateTexture((uint) size.Width, (uint) size.Height, Graphics.PixelFormat.RGBA,
            usage: TextureUsage.Framebuffer, mipmap: false);
        Framebuffer.AttachTexture(InternalTexture);

        if (flags == RenderTargetFlags.DepthStencil)
        {
            _depthTexture = device.CreateTexture((uint) size.Width, (uint) size.Height,
                Graphics.PixelFormat.DepthStencil, TextureSample.Linear, false, TextureUsage.Framebuffer);
            Framebuffer.AttachTexture(_depthTexture);
        }

        Size = size;
    }

    public override void Dispose()
    {
        Framebuffer.Dispose();
        _depthTexture?.Dispose();
        
        base.Dispose();
    }
}

public enum RenderTargetFlags
{
    None,
    DepthStencil
}