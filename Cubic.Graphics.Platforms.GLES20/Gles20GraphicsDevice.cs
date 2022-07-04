using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGLES;

namespace Cubic.Graphics.Platforms.GLES20;

public sealed class Gles20GraphicsDevice : GraphicsDevice
{
    public override event OnViewportResized ViewportResized;
    
    private Rectangle _viewport;
    private Rectangle _scissor;
    private ShaderLayout[] _layout;
    private uint _stride;
    private uint _currentShaderHandle;

    public static GL Gl;

    public Gles20GraphicsDevice(IGLContext context)
    {
        Gl = GL.GetApi(context);
        Options = new Gles20GraphicsDeviceOptions();
        Gl.Enable(EnableCap.Blend);
        Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }
    
    public override GraphicsDeviceOptions Options { get; protected set; }
    
    public override Rectangle Viewport
    {
        get => _viewport;
        set
        {
            _viewport = value;
            Gl.Viewport(value.X, value.Y, (uint) value.Width, (uint) value.Height);
            ViewportResized?.Invoke(value);
        }
    }

    public override Rectangle Scissor
    {
        get => _scissor;
        set
        {
            _scissor = value;
            Gl.Scissor(value.X, _viewport.Height - value.Height - value.Y, (uint) value.Width, (uint) value.Height);
        }
    }

    public override GraphicsApi CurrentApi => GraphicsApi.GLES20;

    public override Buffer CreateBuffer(BufferType type, uint size)
    {
        return new Gles20Buffer(type, size);
    }

    public override Texture CreateTexture(uint width, uint height, PixelFormat format, TextureSample sample = TextureSample.Linear,
        bool mipmap = true, TextureUsage usage = TextureUsage.Texture, TextureWrap wrap = TextureWrap.Repeat, uint anisotropicLevel = 0)
    {
        return new Gles20Texture(width, height, format, sample, mipmap, usage, wrap, anisotropicLevel);
    }

    public override Framebuffer CreateFramebuffer()
    {
        return new Gles20Framebuffer(Gl.GenFramebuffer());
    }

    public override Shader CreateShader(params ShaderAttachment[] attachments)
    {
        return new Gles20Shader(attachments);
    }

    public override byte[] GetPixels(Rectangle region)
    {
        throw new NotImplementedException();
    }

    public override void Clear(Color color)
    {
        Gl.ClearColor(color);
        Gl.Clear((uint) ClearBufferMask.ColorBufferBit | (uint) ClearBufferMask.DepthBufferBit);
    }

    public override void Clear(Vector4 color)
    {
        Gl.ClearColor(color.X, color.Y, color.Z, color.W);
        Gl.Clear((uint) ClearBufferMask.ColorBufferBit | (uint) ClearBufferMask.DepthBufferBit);
    }

    public override void SetShader(Shader shader)
    {
        _currentShaderHandle = ((Gles20Shader) shader).Handle;
        Gl.UseProgram(_currentShaderHandle);
        _layout = ((Gles20Shader) shader).Layout;
        _stride = ((Gles20Shader) shader).Stride;
    }

    public override void SetVertexBuffer(Buffer vertexBuffer)
    {
        Gles20Buffer buf = (Gles20Buffer) vertexBuffer;
        Gl.BindBuffer(buf.Target, buf.Handle);
        SetupAttribs(_stride, _layout);
    }

    public override void SetVertexBuffer(Buffer vertexBuffer, uint stride, params ShaderLayout[] layout)
    {
        Gles20Buffer buf = (Gles20Buffer) vertexBuffer;
        Gl.BindBuffer(buf.Target, buf.Handle);
        SetupAttribs(stride, layout);
    }

    public override void SetIndexBuffer(Buffer indexBuffer)
    {
        Gles20Buffer buf = (Gles20Buffer) indexBuffer;
        Gl.BindBuffer(buf.Target, buf.Handle);
    }

    public override void SetTexture(uint slot, Texture texture)
    {
        Gles20Texture tex = (Gles20Texture) texture;
        TextureTarget target = tex.TextureUsage switch
        {
            TextureUsage.Texture => TextureTarget.Texture2D,
            TextureUsage.Framebuffer => TextureTarget.Texture2D,
            TextureUsage.Cubemap => TextureTarget.TextureCubeMap,
            _ => throw new ArgumentOutOfRangeException(nameof(tex.TextureUsage), tex.TextureUsage, null)
        };
        Gl.ActiveTexture((TextureUnit) ((int) TextureUnit.Texture0 + slot));
        Gl.BindTexture(target, tex.Handle);
    }

    public override void SetTexture(uint slot, IntPtr texture)
    {
        Gl.BindTexture(TextureTarget.Texture2D, (uint) texture.ToInt32());
    }

    public override void SetFramebuffer(Framebuffer framebuffer)
    {
        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, ((Gles20Framebuffer) framebuffer)?.Handle ?? 0);
    }

    public override unsafe void Draw(uint count)
    {
        Gl.DrawElements(PrimitiveType.Triangles, count, DrawElementsType.UnsignedInt, null);
    }

    public override unsafe void Draw(uint count, int indices)
    {
        Gl.DrawElements(PrimitiveType.Triangles, count, DrawElementsType.UnsignedInt, (void*) indices);
    }

    public override unsafe void Draw(uint count, int indices, int baseVertex)
    {
        Gl.DrawElementsBaseVertex(PrimitiveType.Triangles, count, DrawElementsType.UnsignedShort, (void*) indices, baseVertex);
    }

    public override void Dispose()
    {
    }

    private unsafe void SetupAttribs(uint stride, ShaderLayout[] layouts)
    {
        int offset = 0;

        Array.Sort(layouts,
            (layout, shaderLayout) => Gl.GetAttribLocation(_currentShaderHandle, layout.Name)
                .CompareTo(Gl.GetAttribLocation(_currentShaderHandle, shaderLayout.Name)));

        for (int i = 0; i < layouts.Length; i++)
        {
            int size = layouts[i].Size;
            VertexAttribPointerType vType = layouts[i].Type == AttribType.Byte
                ? VertexAttribPointerType.UnsignedByte
                : VertexAttribPointerType.Float;
            Gl.EnableVertexAttribArray((uint) i);
            Gl.VertexAttribPointer((uint) i, size, vType, layouts[i].Normalize, stride, (void*) offset);
            offset += size * 4;
        }
    }
}