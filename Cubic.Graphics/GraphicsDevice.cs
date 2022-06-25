using System;
using System.Drawing;
using System.Numerics;

namespace Cubic.Graphics;

public abstract class GraphicsDevice : IDisposable
{
    public abstract Rectangle Viewport { get; set; }
    
    public abstract Rectangle Scissor { get; set; }
    
    public abstract Buffer CreateBuffer(BufferType type, uint size);

    public abstract void UpdateBuffer<T>(Buffer buffer, int offset, T[] data) where T : unmanaged;

    public abstract Texture CreateTexture(uint width, uint height, PixelFormat format);

    public abstract void UpdateTexture<T>(Texture texture, int x, int y, uint width, uint height, T[] data) where T : unmanaged;

    public abstract ShaderProgram CreateShaderProgram(params Shader[] shaders);

    public abstract Shader CreateShader(ShaderType type, string code);

    public abstract void Clear(Color color);

    public abstract void Clear(Vector4 color);

    public abstract void SetUniform(ShaderProgram program, string uniformName, bool value);

    public abstract void SetUniform(ShaderProgram program, string uniformName, int value);

    public abstract void SetUniform(ShaderProgram program, string uniformName, float value);

    public abstract void SetUniform(ShaderProgram program, string uniformName, Vector2 value);
    
    public abstract void SetUniform(ShaderProgram program, string uniformName, Vector3 value);
    
    public abstract void SetUniform(ShaderProgram program, string uniformName, Vector4 value);

    public abstract void SetUniform(ShaderProgram program, string uniformName, Color color);

    public abstract void SetUniform(ShaderProgram program, string uniformName, Matrix4x4 matrix, bool transpose = true);

    public abstract void SetShaderProgram(ShaderProgram program);

    public abstract void SetVertexBuffer(Buffer vertexBuffer);

    public abstract void SetIndexBuffer(Buffer indexBuffer);

    public abstract void SetTexture(Texture texture);

    public abstract void DrawElements(uint count);

    public abstract void Dispose();
}