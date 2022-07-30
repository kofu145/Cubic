using System;
using System.Drawing;
using System.Numerics;

namespace Cubic.Graphics;

/// <summary>
/// Represents a shader/effect that can be used during rendering.
/// </summary>
public abstract class Shader : IDisposable
{
    /// <summary>
    /// Will return <see langword="true" /> when this shader has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// Set a bool uniform with the given name and value.
    /// </summary>
    /// <param name="uniformName">The name of the uniform.</param>
    /// <param name="value">The value to set.</param>
    public abstract void SetUniform(string uniformName, bool value);

    /// <summary>
    /// Set an int uniform with the given name and value.
    /// </summary>
    /// <param name="uniformName">The name of the uniform.</param>
    /// <param name="value">The value to set.</param>
    public abstract void SetUniform(string uniformName, int value);

    /// <summary>
    /// Set a float uniform with the given name and value.
    /// </summary>
    /// <param name="uniformName">The name of the uniform.</param>
    /// <param name="value">The value to set.</param>
    public abstract void SetUniform(string uniformName, float value);

    /// <summary>
    /// Set a vec2 uniform with the given name and value.
    /// </summary>
    /// <param name="uniformName">The name of the uniform.</param>
    /// <param name="value">The value to set.</param>
    public abstract void SetUniform(string uniformName, Vector2 value);
    
    /// <summary>
    /// Set a vec3 uniform with the given name and value.
    /// </summary>
    /// <param name="uniformName">The name of the uniform.</param>
    /// <param name="value">The value to set.</param>
    public abstract void SetUniform(string uniformName, Vector3 value);
    
    /// <summary>
    /// Set a vec4 uniform with the given name and value.
    /// </summary>
    /// <param name="uniformName">The name of the uniform.</param>
    /// <param name="value">The value to set.</param>
    public abstract void SetUniform(string uniformName, Vector4 value);

    /// <summary>
    /// Set a vec4 uniform with the given name and color value. NOTE: This is a helper method, it will normalize the color
    /// and set a vec4 uniform.
    /// </summary>
    /// <param name="uniformName">The name of the uniform.</param>
    /// <param name="color">The color to normalize and set.</param>
    public abstract void SetUniform(string uniformName, Color color);

    /// <summary>
    /// Set a mat4 uniform with the given name and value.
    /// </summary>
    /// <param name="uniformName">The name of the uniform.</param>
    /// <param name="matrix">The matrix itself.</param>
    /// <param name="transpose">Whether or not to transpose this matrix. If using System.Numerics, you should generally
    /// leave this value as true if passing in a matrix directly.</param>
    public abstract void SetUniform(string uniformName, Matrix4x4 matrix, bool transpose = true);
    
    /// <summary>
    /// Dispose this shader.
    /// </summary>
    public abstract void Dispose();
}