using static Cubic.Graphics.Platforms.OpenGL33.OpenGL33GraphicsDevice;

namespace Cubic.Graphics.Platforms.OpenGL33;

public class OpenGL33Texture : Texture
{
    public uint Handle;
    public Silk.NET.OpenGL.PixelFormat Format;
    public TextureUsage Usage;
    public bool Mipmap;

    internal OpenGL33Texture(uint handle, Silk.NET.OpenGL.PixelFormat format, TextureUsage usage, bool mipmap)
    {
        Handle = handle;
        Format = format;
        Usage = usage;
        Mipmap = mipmap;
    }
    
    public override void Dispose()
    {
        Gl.DeleteTexture(Handle);
    }
}