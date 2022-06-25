using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Text;
using Cubic.Graphics;
using Cubic.Utilities;
using Cubic.Windowing;
using static Cubic.Render.GraphicsMachine;
using ShaderType = Cubic.Graphics.ShaderType;

namespace Cubic.Render;

public class Shader : IDisposable
{
    internal ShaderProgram Program;

    private Dictionary<string, int> _uniformLocations;

    public Shader(string vertex, string fragment, ShaderLoadType loadType = ShaderLoadType.String)
    {
        GraphicsDevice device = GraphicsMachine.GraphicsDevice;
        using Cubic.Graphics.Shader vShader = device.CreateShader(ShaderType.Vertex, vertex);
        using Cubic.Graphics.Shader fShader = device.CreateShader(ShaderType.Fragment, fragment);
        Program = device.CreateShaderProgram(vShader, fShader);
    }

    public void Set(string uniformName, bool value)
    {
        GraphicsMachine.GraphicsDevice.SetUniform(Program, uniformName, value);
    }
    
    public void Set(string uniformName, int value)
    {
        GraphicsMachine.GraphicsDevice.SetUniform(Program, uniformName, value);
    }
    
    public void Set(string uniformName, float value)
    {
        GraphicsMachine.GraphicsDevice.SetUniform(Program, uniformName, value);
    }

    public void Set(string uniformName, Vector2 value)
    {
        GraphicsMachine.GraphicsDevice.SetUniform(Program, uniformName, value);
    }
    
    public void Set(string uniformName, Vector3 value)
    {
        GraphicsMachine.GraphicsDevice.SetUniform(Program, uniformName, value);
    }
    
    public void Set(string uniformName, Vector4 value)
    {
        GraphicsMachine.GraphicsDevice.SetUniform(Program, uniformName, value);
    }

    public void Set(string uniformName, Color color)
    {
        GraphicsMachine.GraphicsDevice.SetUniform(Program, uniformName, color);
    }

    public unsafe void Set(string uniformName, Matrix4x4 value, bool transpose = true)
    {
        GraphicsMachine.GraphicsDevice.SetUniform(Program, uniformName, value, transpose);
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