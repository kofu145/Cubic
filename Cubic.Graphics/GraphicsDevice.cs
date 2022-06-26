using System;
using System.Drawing;
using System.Numerics;

namespace Cubic.Graphics;

public abstract class GraphicsDevice : IDisposable
{
    public abstract event OnViewportResized ViewportResized;
    
    public abstract GraphicsDeviceOptions Options { get; protected set; }

    public abstract Rectangle Viewport { get; set; }
    
    public abstract Rectangle Scissor { get; set; }
    
    public abstract Buffer CreateBuffer(BufferType type, uint size);

    public abstract Texture CreateTexture(uint width, uint height, PixelFormat format,
        TextureSample sample = TextureSample.Linear, bool mipmap = true, TextureUsage usage = TextureUsage.Texture, TextureWrap wrap = TextureWrap.Repeat);

    public abstract Framebuffer CreateFramebuffer();

    public abstract Shader CreateShader(params ShaderAttachment[] attachments);

    public abstract void Clear(Color color);

    public abstract void Clear(Vector4 color);

    public abstract void SetShader(Shader program);

    public abstract void SetVertexBuffer(Buffer vertexBuffer);

    public abstract void SetIndexBuffer(Buffer indexBuffer);

    public abstract void SetTexture(uint slot, Texture texture);

    public abstract void SetFramebuffer(Framebuffer framebuffer);

    public abstract void DrawElements(uint count);

    public abstract void Dispose();
    
    public delegate void OnViewportResized(Rectangle viewport);
}