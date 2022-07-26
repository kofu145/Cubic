using System;
using System.Drawing;
using System.Numerics;
using Cubic.Debugging;
using Cubic.Graphics;

namespace Cubic.Render;

public class Shader : IDisposable
{
    public Cubic.Graphics.Shader InternalProgram;

    public Shader(string vertex, string fragment, ShaderLoadType loadType = ShaderLoadType.String)
    {
        GraphicsDevice device = CubicGraphics.GraphicsDevice;
        InternalProgram = device.CreateShader(new ShaderAttachment(AttachmentType.Vertex, vertex),
            new ShaderAttachment(AttachmentType.Fragment, fragment));
    }

    public void Set(string uniformName, bool value)
    {
        InternalProgram.SetUniform(uniformName, value);
    }
    
    public void Set(string uniformName, int value)
    {
        InternalProgram.SetUniform(uniformName, value);
    }
    
    public void Set(string uniformName, float value)
    {
        InternalProgram.SetUniform(uniformName, value);
    }

    public void Set(string uniformName, Vector2 value)
    {
        InternalProgram.SetUniform(uniformName, value);
    }
    
    public void Set(string uniformName, Vector3 value)
    {
        InternalProgram.SetUniform(uniformName, value);
    }
    
    public void Set(string uniformName, Vector4 value)
    {
        InternalProgram.SetUniform(uniformName, value);
    }

    public void Set(string uniformName, Color color)
    {
        InternalProgram.SetUniform(uniformName, color);
    }

    public unsafe void Set(string uniformName, Matrix4x4 value, bool transpose = true)
    {
        InternalProgram.SetUniform(uniformName, value, transpose);
    }

    public void Dispose()
    {
        InternalProgram.Dispose();

#if DEBUG
        CubicDebug.WriteLine("Shader disposed");
#endif
    }
}

public enum ShaderLoadType
{
    File,
    String
}