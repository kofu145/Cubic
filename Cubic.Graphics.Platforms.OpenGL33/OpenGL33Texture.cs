using static Cubic.Graphics.Platforms.OpenGL33.OpenGL33GraphicsDevice;

namespace Cubic.Graphics.Platforms.OpenGL33;

public class OpenGL33Texture : Texture
{
    public uint Handle;
    public Silk.NET.OpenGL.PixelFormat Format;

    public OpenGL33Texture(uint handle, Silk.NET.OpenGL.PixelFormat format)
    {
        Handle = handle;
        Format = format;
    }
    
    public override void Dispose()
    {
        Gl.DeleteTexture(Handle);
    }
}