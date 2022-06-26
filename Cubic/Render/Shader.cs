using System;
using System.Drawing;
using System.Numerics;
using Cubic.Graphics;

namespace Cubic.Render;

public class Shader : IDisposable
{
    internal Cubic.Graphics.Shader Program;

    public Shader(string vertex, string fragment, ShaderLoadType loadType = ShaderLoadType.String)
    {
        GraphicsDevice device = CubicGraphics.GraphicsDevice;
        Program = device.CreateShader(new ShaderAttachment(AttachmentType.Vertex, vertex),
            new ShaderAttachment(AttachmentType.Fragment, fragment));
    }

    public void Set(string uniformName, bool value)
    {
        Program.SetUniform(uniformName, value);
    }
    
    public void Set(string uniformName, int value)
    {
        Program.SetUniform(uniformName, value);
    }
    
    public void Set(string uniformName, float value)
    {
        Program.SetUniform(uniformName, value);
    }

    public void Set(string uniformName, Vector2 value)
    {
        Program.SetUniform(uniformName, value);
    }
    
    public void Set(string uniformName, Vector3 value)
    {
        Program.SetUniform(uniformName, value);
    }
    
    public void Set(string uniformName, Vector4 value)
    {
        Program.SetUniform(uniformName, value);
    }

    public void Set(string uniformName, Color color)
    {
        Program.SetUniform(uniformName, color);
    }

    public unsafe void Set(string uniformName, Matrix4x4 value, bool transpose = true)
    {
        Program.SetUniform(uniformName, value, transpose);
    }

    public void Dispose()
    {
        Program.Dispose();

#if DEBUG
        Console.WriteLine("Shader disposed");
#endif
    }
}

public enum ShaderLoadType
{
    File,
    String
}