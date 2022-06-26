using System;
using System.Drawing;
using System.Numerics;

namespace Cubic.Graphics;

public abstract class GraphicsDevice : IDisposable
{
    public abstract event OnViewportResized ViewportResized;
    
    public abstract DepthTest DepthTest { get; set; }
    
    public abstract bool EnableScissor { get; set; }
    
    public abstract bool DepthMask { get; set; }
    
    public abstract CullFace CullFace { get; set; }
    
    public abstract CullDirection CullDirection { get; set; }
    
    public abstract Rectangle Viewport { get; set; }
    
    public abstract Rectangle Scissor { get; set; }
    
    public abstract Buffer CreateBuffer(BufferType type, uint size);

    public abstract void UpdateBuffer<T>(Buffer buffer, int offset, T[] data) where T : unmanaged;

    public abstract Texture CreateTexture(uint width, uint height, PixelFormat format,
        TextureSample sample = TextureSample.Linear, bool mipmap = true, TextureUsage usage = TextureUsage.Texture);

    public abstract Framebuffer CreateFramebuffer();

    public abstract void AttachTextureToFramebuffer(Framebuffer framebuffer, Texture texture, int colorAttachment = 0);

    public abstract void UpdateTexture<T>(Texture texture, int x, int y, uint width, uint height, T[] data) where T : unmanaged;

    public abstract void UpdateTexture<T>(Texture texture, int x, int y, uint width, uint height, T[] data,
        CubemapPosition position) where T : unmanaged;

    public abstract void UpdateTexture(Texture texture, int x, int y, uint width, uint height, IntPtr data);

    public abstract void SetTextureSample(Texture texture, TextureSample sample);

    public abstract void SetTextureUsage(Texture texture, TextureUsage usage);

    public abstract void GenerateTextureMipmaps(Texture texture);

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

    public abstract void SetTexture(uint slot, Texture texture);

    public abstract void SetFramebuffer(Framebuffer framebuffer);

    public abstract void DrawElements(uint count);

    public abstract void Dispose();
    
    public delegate void OnViewportResized(Rectangle viewport);
}