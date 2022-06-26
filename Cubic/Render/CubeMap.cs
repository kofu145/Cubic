using Cubic.Graphics;
using Cubic.Utilities;
using Silk.NET.OpenGL;
using PixelFormat = Cubic.Graphics.PixelFormat;

namespace Cubic.Render;

public class CubeMap : Texture
{
    public CubeMap(Bitmap top, Bitmap bottom, Bitmap front, Bitmap back, Bitmap right, Bitmap left, bool autoDispose = true) : base(autoDispose)
    {
        GraphicsDevice device = CubicGraphics.GraphicsDevice;
        Tex = device.CreateTexture((uint) top.Size.Width, (uint) top.Size.Height, PixelFormat.RGBA,
            usage: TextureUsage.Cubemap);
        Tex.Update(0, 0, (uint) top.Size.Width, (uint) top.Size.Height, top.Data, CubemapPosition.PositiveY);
        Tex.Update(0, 0, (uint) bottom.Size.Width, (uint) bottom.Size.Height, bottom.Data, CubemapPosition.NegativeY);
        Tex.Update(0, 0, (uint) front.Size.Width, (uint) front.Size.Height, front.Data, CubemapPosition.PositiveZ);
        Tex.Update(0, 0, (uint) back.Size.Width, (uint) back.Size.Height, back.Data, CubemapPosition.NegativeZ);
        Tex.Update(0, 0, (uint) right.Size.Width, (uint) right.Size.Height, right.Data, CubemapPosition.PositiveX);
        Tex.Update(0, 0, (uint) left.Size.Width, (uint) left.Size.Height, left.Data, CubemapPosition.NegativeX);
    }
}