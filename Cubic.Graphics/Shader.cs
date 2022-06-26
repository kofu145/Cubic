using System;
using System.Drawing;
using System.Numerics;

namespace Cubic.Graphics;

public abstract class Shader : IDisposable
{
    public abstract void SetUniform(string uniformName, bool value);

    public abstract void SetUniform(string uniformName, int value);

    public abstract void SetUniform(string uniformName, float value);

    public abstract void SetUniform(string uniformName, Vector2 value);
    
    public abstract void SetUniform(string uniformName, Vector3 value);
    
    public abstract void SetUniform(string uniformName, Vector4 value);

    public abstract void SetUniform(string uniformName, Color color);

    public abstract void SetUniform(string uniformName, Matrix4x4 matrix, bool transpose = true);
    
    public abstract void Dispose();
}