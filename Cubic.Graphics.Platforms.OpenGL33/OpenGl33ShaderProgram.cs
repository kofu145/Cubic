using static Cubic.Graphics.Platforms.OpenGL33.OpenGL33GraphicsDevice;

namespace Cubic.Graphics.Platforms.OpenGL33;

public class OpenGl33ShaderProgram : ShaderProgram
{
    public uint Handle;
    
    public OpenGl33ShaderProgram(uint handle)
    {
        Handle = handle;
    }
    
    public override void Dispose()
    {
        Gl.DeleteProgram(Handle);
    }
}