using static Cubic.Graphics.Platforms.OpenGL33.OpenGL33GraphicsDevice;

namespace Cubic.Graphics.Platforms.OpenGL33;

public class OpenGL33Framebuffer : Framebuffer
{
    public uint Handle;
    
    internal OpenGL33Framebuffer(uint handle)
    {
        Handle = handle;
    }

    public override void Dispose()
    {
        Gl.DeleteFramebuffer(Handle);
    }
}