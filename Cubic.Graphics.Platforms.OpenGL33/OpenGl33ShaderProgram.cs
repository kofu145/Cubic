using System.Collections.Generic;
using static Cubic.Graphics.Platforms.OpenGL33.OpenGL33GraphicsDevice;

namespace Cubic.Graphics.Platforms.OpenGL33;

public class OpenGl33ShaderProgram : ShaderProgram
{
    public uint Handle;
    public Dictionary<string, int> UniformLocations;

    internal OpenGl33ShaderProgram(uint handle, Dictionary<string, int> uniformLocations)
    {
        Handle = handle;
        UniformLocations = uniformLocations;
    }
    
    public override void Dispose()
    {
        Gl.DeleteProgram(Handle);
    }
}