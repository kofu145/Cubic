using System.Drawing;
using Cubic.Graphics;
using Silk.NET.OpenGL;
using Framebuffer = Cubic.Graphics.Framebuffer;
using PixelFormat = Silk.NET.OpenGL.PixelFormat;

namespace Cubic.Render;

public class RenderTarget : Texture
{
    internal Framebuffer Framebuffer;

    public unsafe RenderTarget(Size size, bool autoDispose) : base(autoDispose)
    {
        GraphicsDevice device = CubicGraphics.GraphicsDevice;

        Framebuffer = device.CreateFramebuffer();
        Tex = device.CreateTexture((uint) size.Width, (uint) size.Height, Graphics.PixelFormat.RGBA,
            usage: TextureUsage.Framebuffer, mipmap: false);
        Framebuffer.AttachTexture(Tex);

        Size = size;
    }

    public override void Dispose()
    {
        Framebuffer.Dispose();
        
        base.Dispose();
    }
}