using static Cubic.Graphics.Platforms.OpenGL33.OpenGL33GraphicsDevice;

namespace Cubic.Graphics.Platforms.OpenGL33;

public class OpenGL33Shader : Shader
{
    public uint Handle;

    public OpenGL33Shader(uint handle)
    {
        Handle = handle;
    }

    public override void Dispose()
    {
        Gl.DeleteShader(Handle);
    }
}