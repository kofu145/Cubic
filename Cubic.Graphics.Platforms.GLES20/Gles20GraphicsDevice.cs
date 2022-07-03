using System;
using System.Drawing;
using System.Numerics;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGLES;

namespace Cubic.Graphics.Platforms.GLES20;

public sealed class Gles20GraphicsDevice : GraphicsDevice
{
    public override event OnViewportResized ViewportResized;

    public static GL Gl;

    public Gles20GraphicsDevice(IGLContext context)
    {
        Gl = GL.GetApi(context);
        Options = new Gles20GraphicsDeviceOptions();
        Gl.Enable(EnableCap.Blend);
        Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }
    
    public override GraphicsDeviceOptions Options { get; protected set; }
    public override Rectangle Viewport { get; set; }
    public override Rectangle Scissor { get; set; }
    public override Buffer CreateBuffer(BufferType type, uint size)
    {
        return new Gles20Buffer(type, size);
    }

    public override Texture CreateTexture(uint width, uint height, PixelFormat format, TextureSample sample = TextureSample.Linear,
        bool mipmap = true, TextureUsage usage = TextureUsage.Texture, TextureWrap wrap = TextureWrap.Repeat, uint anisotropicLevel = 0)
    {
        throw new NotImplementedException();
    }

    public override Framebuffer CreateFramebuffer()
    {
        throw new NotImplementedException();
    }

    public override Shader CreateShader(params ShaderAttachment[] attachments)
    {
        throw new NotImplementedException();
    }

    public override byte[] GetPixels(Rectangle region)
    {
        throw new NotImplementedException();
    }

    public override void Clear(Color color)
    {
        throw new NotImplementedException();
    }

    public override void Clear(Vector4 color)
    {
        throw new NotImplementedException();
    }

    public override void SetShader(Shader program)
    {
        throw new NotImplementedException();
    }

    public override void SetVertexBuffer(Buffer vertexBuffer)
    {
        throw new NotImplementedException();
    }

    public override void SetVertexBuffer(Buffer vertexBuffer, uint stride, params ShaderLayout[] layout)
    {
        throw new NotImplementedException();
    }

    public override void SetIndexBuffer(Buffer indexBuffer)
    {
        throw new NotImplementedException();
    }

    public override void SetTexture(uint slot, Texture texture)
    {
        throw new NotImplementedException();
    }

    public override void SetTexture(uint slot, IntPtr texture)
    {
        throw new NotImplementedException();
    }

    public override void SetFramebuffer(Framebuffer framebuffer)
    {
        throw new NotImplementedException();
    }

    public override void Draw(uint count)
    {
        throw new NotImplementedException();
    }

    public override void Draw(uint count, int indices)
    {
        throw new NotImplementedException();
    }

    public override void Draw(uint count, int indices, int baseVertex)
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}