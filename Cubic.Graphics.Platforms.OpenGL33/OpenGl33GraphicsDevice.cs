﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGL;

namespace Cubic.Graphics.Platforms.OpenGL33;

public sealed class OpenGl33GraphicsDevice : GraphicsDevice
{
    public override event OnViewportResized ViewportResized;
    public override GraphicsDeviceOptions Options { get; protected set; }

    public static GL Gl;
    private uint _vao;
    private Rectangle _viewport;
    private Rectangle _scissor;
    private Dictionary<Type, AttribSetup> _attribsCache;

    private IGLContext _context;
    
    public OpenGl33GraphicsDevice(IGLContext context)
    {
        Gl = GL.GetApi(context);
        _vao = Gl.GenVertexArray();
        Gl.BindVertexArray(_vao);
        _context = context;
        _attribsCache = new Dictionary<Type, AttribSetup>();
        Options = new OpenGl33GraphicsDeviceOptions();
        
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

    public override GraphicsApi CurrentApi => GraphicsApi.OpenGL33;

    public override Buffer CreateBuffer(BufferType type, uint size)
    {
        return new OpenGl33Buffer(type, size);
    }

    public override Texture CreateTexture(uint width, uint height, PixelFormat format, TextureSample sample = TextureSample.Linear, bool mipmap = true, TextureUsage usage = TextureUsage.Texture, TextureWrap wrap = TextureWrap.Repeat, uint anisotropicLevel = 0)
    {
        return new OpenGl33Texture(width, height, format, sample, mipmap, usage, wrap, anisotropicLevel);
    }

    public override Framebuffer CreateFramebuffer()
    {
        return new OpenGl33Framebuffer(Gl.CreateFramebuffer());
    }

    public override Shader CreateShader(params ShaderAttachment[] attachments)
    {
        return new OpenGl33Shader(attachments);
    }

    public override unsafe byte[] GetPixels(Rectangle region)
    {
        byte[] pixels = new byte[region.Width * region.Height * 4];
        fixed (byte* pixelPointer = pixels)
            Gl.ReadPixels(region.X, region.Y, (uint) region.Width, (uint) region.Height, Silk.NET.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, pixelPointer);

        for (int x = 0; x < region.Width; x++)
        {
            for (int y = 0; y < region.Height / 2; y++)
            {
                int loc = (y * region.Width + x) * 4;
                int invLoc = ((region.Height - 1 - y) * region.Width + x) * 4;
                byte r = pixels[invLoc];
                byte g = pixels[invLoc + 1];
                byte b = pixels[invLoc + 2];
                pixels[invLoc] = pixels[loc];
                pixels[invLoc + 1] = pixels[loc + 1];
                pixels[invLoc + 2] = pixels[loc + 2];
                pixels[invLoc + 3] = 255;
                pixels[loc] = r;
                pixels[loc + 1] = g;
                pixels[loc + 2] = b;
                pixels[loc + 3] = 255;
            }
        }
        
        return pixels;
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
        Gl.UseProgram(((OpenGl33Shader) shader).Handle);
    }

    public override void SetVertexBuffer(Buffer vertexBuffer)
    {
        OpenGl33Buffer buf = (OpenGl33Buffer) vertexBuffer;
        Gl.BindBuffer(buf.Target, buf.Handle);
        SetupAttribs(buf.Type);
    }

    public override void SetVertexBuffer(Buffer vertexBuffer, uint stride, params ShaderLayout[] layout)
    {
        OpenGl33Buffer buf = (OpenGl33Buffer) vertexBuffer;
        Gl.BindBuffer(buf.Target, buf.Handle);
        SetupAttribs(stride, layout);
    }

    public override void SetIndexBuffer(Buffer indexBuffer)
    {
        OpenGl33Buffer buf = (OpenGl33Buffer) indexBuffer;
        Gl.BindBuffer(buf.Target, buf.Handle);
    }

    public override void SetTexture(uint slot, Texture texture)
    {
        OpenGl33Texture tex = (OpenGl33Texture) texture;
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
        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, ((OpenGl33Framebuffer) framebuffer)?.Handle ?? 0);
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
        Gl.DeleteVertexArray(_vao);
    }

    private unsafe void SetupAttribs(Type type)
    {
        if (!_attribsCache.TryGetValue(type, out AttribSetup setup))
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            uint totalSizeInBytes = 0;
            List<int> sizes = new List<int>();
            List<VertexAttribPointerType> types = new List<VertexAttribPointerType>();
            foreach (FieldInfo info in fields)
            {
                VertexAttribPointerType pointerType = VertexAttribPointerType.Float;
                Attribute attrib = info.GetCustomAttribute(typeof(AttribTypeAttribute));
                if (attrib != null)
                {
                    Console.WriteLine(((AttribTypeAttribute) attrib).Type);
                    pointerType = ((AttribTypeAttribute) attrib).Type == AttribType.Byte
                        ? VertexAttribPointerType.UnsignedByte
                        : VertexAttribPointerType.Float;
                }
                int size = Marshal.SizeOf(info.FieldType);
                sizes.Add(size);
                totalSizeInBytes += (uint) size;
                types.Add(pointerType);
            }
            
            Console.WriteLine($"Create attrib of type {type}");
            
            setup = new AttribSetup(totalSizeInBytes, sizes, types);
            _attribsCache.Add(type, setup);
        }
        
        uint location = 0;
        int offset = 0;

        for (int i = 0; i < setup.Sizes.Length; i++)
        {
            int size = setup.Sizes[i];
            VertexAttribPointerType vType = setup.Types[i];
            Gl.EnableVertexAttribArray(location);
            Gl.VertexAttribPointer(location, size / 4, vType, false, setup.TotalSize, (void*) offset);
            offset += size;
            location += 1;
        }
    }

    private unsafe void SetupAttribs(uint stride, ShaderLayout[] layouts)
    {
        uint location = 0;
        int offset = 0;

        for (int i = 0; i < layouts.Length; i++)
        {
            int size = layouts[i].Size;
            VertexAttribPointerType vType = layouts[i].Type == AttribType.Byte
                ? VertexAttribPointerType.UnsignedByte
                : VertexAttribPointerType.Float;
            Gl.EnableVertexAttribArray(location);
            Gl.VertexAttribPointer(location, size, vType, layouts[i].Normalize, stride, (void*) offset);
            offset += size * 4;
            location += 1;
        }
    }
}