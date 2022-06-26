using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGL;

namespace Cubic.Graphics.Platforms.OpenGL33;

public sealed class OpenGL33GraphicsDevice : GraphicsDevice
{
    public override event OnViewportResized ViewportResized;
    public override GraphicsDeviceOptions Options { get; protected set; }

    public static GL Gl;
    private uint _vao;
    private Rectangle _viewport;
    private Rectangle _scissor;
    private Dictionary<Type, AttribSetup> _attribsCache;

    private IGLContext _context;
    
    public OpenGL33GraphicsDevice(IGLContext context)
    {
        Gl = GL.GetApi(context);
        _vao = Gl.GenVertexArray();
        Gl.BindVertexArray(_vao);
        _context = context;
        _attribsCache = new Dictionary<Type, AttribSetup>();
        Options = new OpenGL33GraphicsDeviceOptions();
        
        // TODO: This
        Gl.Enable(EnableCap.Blend);
        Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

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

    public override Buffer CreateBuffer(BufferType type, uint size)
    {
        return new OpenGL33Buffer(type, size);
    }

    public override Texture CreateTexture(uint width, uint height, PixelFormat format, TextureSample sample = TextureSample.Linear, bool mipmap = true, TextureUsage usage = TextureUsage.Texture, TextureWrap wrap = TextureWrap.Repeat)
    {
        return new OpenGL33Texture(width, height, format, sample, mipmap, usage, wrap);
    }

    public override Framebuffer CreateFramebuffer()
    {
        return new OpenGL33Framebuffer(Gl.CreateFramebuffer());
    }

    public override Shader CreateShader(params ShaderAttachment[] attachments)
    {
        return new OpenGL33Shader(attachments);
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
        Gl.UseProgram(((OpenGL33Shader) shader).Handle);
    }

    public override void SetVertexBuffer(Buffer vertexBuffer)
    {
        OpenGL33Buffer buf = (OpenGL33Buffer) vertexBuffer;
        Gl.BindBuffer(buf.Target, buf.Handle);
        SetupAttribs(buf.Type);
    }

    public override void SetIndexBuffer(Buffer indexBuffer)
    {
        OpenGL33Buffer buf = (OpenGL33Buffer) indexBuffer;
        Gl.BindBuffer(buf.Target, buf.Handle);
    }

    public override void SetTexture(uint slot, Texture texture)
    {
        OpenGL33Texture tex = (OpenGL33Texture) texture;
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

    public override void SetFramebuffer(Framebuffer framebuffer)
    {
        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, ((OpenGL33Framebuffer) framebuffer)?.Handle ?? 0);
    }

    public override unsafe void DrawElements(uint count)
    {
        Gl.DrawElements(PrimitiveType.Triangles, count, DrawElementsType.UnsignedInt, null);
    }

    public override void Dispose()
    {
        Gl.DeleteVertexArray(_vao);
    }

    private unsafe void SetupAttribs(Type type)
    {
        if (!_attribsCache.TryGetValue(type, out AttribSetup setup))
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            uint totalSizeInBytes = 0;
            List<int> sizes = new List<int>();
            foreach (FieldInfo info in fields)
            {
                int size = Marshal.SizeOf(info.FieldType);
                sizes.Add(size);
                totalSizeInBytes += (uint) size;
            }
            
            Console.WriteLine($"Create attrib of type {type}");
            
            setup = new AttribSetup(totalSizeInBytes, sizes);
            _attribsCache.Add(type, setup);
        }
        
        uint location = 0;
        int offset = 0;

        foreach (int size in setup.Sizes)
        {
            Gl.EnableVertexAttribArray(location);
            Gl.VertexAttribPointer(location, size / 4, VertexAttribPointerType.Float, false, setup.TotalSize, (void*) offset);
            offset += size;
            location += 1;
        }
    }
}