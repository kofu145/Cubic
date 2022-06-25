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
    public static GL Gl;
    private uint _vao;
    
    public OpenGL33GraphicsDevice(IGLContext context)
    {
        Gl = GL.GetApi(context);
        _vao = Gl.GenVertexArray();
        Gl.BindVertexArray(_vao);
    }

    public override Rectangle Viewport { get; set; }
    public override Rectangle Scissor { get; set; }

    public override unsafe Buffer CreateBuffer(BufferType type, uint size)
    {
        uint handle = Gl.GenBuffer();
        BufferTargetARB target = type switch
        {
            BufferType.VertexBuffer => BufferTargetARB.ArrayBuffer,
            BufferType.IndexBuffer => BufferTargetARB.ElementArrayBuffer,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        Gl.BindBuffer(target, handle);
        Gl.BufferData(target, size, null, BufferUsageARB.DynamicDraw);
        return new OpenGL33Buffer(handle, target);
    }

    public override unsafe void UpdateBuffer<T>(Buffer buffer, int offset, T[] data)
    {
        OpenGL33Buffer buf = (OpenGL33Buffer) buffer;
        Gl.BindBuffer(buf.Target, buf.Handle);
        fixed (void* dat = data)
            Gl.BufferSubData(buf.Target, offset, (nuint) (data.Length * Unsafe.SizeOf<T>()), dat);
        buf.Type = data.GetType();
    }

    public override unsafe Texture CreateTexture(uint width, uint height, PixelFormat format)
    {
        uint handle = Gl.GenTexture();
        Gl.BindTexture(TextureTarget.Texture2D, handle);
        Silk.NET.OpenGL.PixelFormat fmt = format switch
        {
            PixelFormat.RGB => Silk.NET.OpenGL.PixelFormat.Rgb,
            PixelFormat.RGBA => Silk.NET.OpenGL.PixelFormat.Rgba,
            PixelFormat.BRGA => Silk.NET.OpenGL.PixelFormat.Bgra,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
        Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, width, height, 0, fmt, PixelType.UnsignedByte,
            null);
        return new OpenGL33Texture(handle, fmt);
    }

    public override unsafe void UpdateTexture<T>(Texture texture, int x, int y, uint width, uint height, T[] data)
    {
        OpenGL33Texture tex = (OpenGL33Texture) texture;
        
        Gl.BindTexture(TextureTarget.Texture2D, tex.Handle);
        fixed (void* dat = data)
            Gl.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, width, height, tex.Format, PixelType.UnsignedByte, dat);
    }

    public override ShaderProgram CreateShaderProgram(params Shader[] shaders)
    {
        uint handle = Gl.CreateProgram();
        foreach (OpenGL33Shader shader in shaders)
            Gl.AttachShader(handle, shader.Handle);
        Gl.LinkProgram(handle);
        Gl.GetProgram(handle, ProgramPropertyARB.LinkStatus, out int status);
        if (status != (int) GLEnum.True)
            throw new GraphicsException($"Error linking program.\n\n{Gl.GetProgramInfoLog(handle)}");
        foreach (OpenGL33Shader shader in shaders)
            Gl.DetachShader(handle, shader.Handle);

        return new OpenGl33ShaderProgram(handle);
    }

    public override Shader CreateShader(ShaderType type, string code)
    {
        Silk.NET.OpenGL.ShaderType sType = type switch
        {
            ShaderType.Vertex => Silk.NET.OpenGL.ShaderType.VertexShader,
            ShaderType.Fragment => Silk.NET.OpenGL.ShaderType.FragmentShader,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        code = code.Insert(0, "#version 330 core\n");

        uint s = Gl.CreateShader(sType);
        Gl.ShaderSource(s, code);
        Gl.CompileShader(s);
        Gl.GetShader(s, ShaderParameterName.CompileStatus, out int status);
        if (status != (int) GLEnum.True)
            throw new GraphicsException($"Error compiling shader.\n\n{Gl.GetShaderInfoLog(s)}");

        return new OpenGL33Shader(s);
    }

    public override void Clear(Color color)
    {
        Gl.ClearColor(color);
        Gl.Clear((uint) ClearBufferMask.ColorBufferBit | (uint) ClearBufferMask.DepthBufferBit);
    }

    public override void Clear(Vector4 color)
    {
        Gl.ClearColor(color.X / 255f, color.Y / 255f, color.Z / 255f, color.W / 255f);
        Gl.Clear((uint) ClearBufferMask.ColorBufferBit | (uint) ClearBufferMask.DepthBufferBit);
    }

    public override void SetShaderProgram(ShaderProgram program)
    {
        Gl.UseProgram(((OpenGl33ShaderProgram) program).Handle);
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

    public override void SetTexture(Texture texture)
    {
        Gl.ActiveTexture(TextureUnit.Texture0);
        Gl.BindTexture(TextureTarget.Texture2D, ((OpenGL33Texture) texture).Handle);
    }

    public override unsafe void DrawElements(uint count)
    {
        Gl.DrawElements(PrimitiveType.Triangles, count, DrawElementsType.UnsignedShort, null);
    }

    public override void Dispose()
    {
        Gl.DeleteVertexArray(_vao);
    }

    private static unsafe void SetupAttribs(Type type)
    {
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        uint location = 0;
        int offset = 0;
        uint totalSizeInBytes = 0;
        List<int> sizes = new List<int>();
        foreach (FieldInfo info in fields)
        {
            int size = Marshal.SizeOf(info.FieldType);
            sizes.Add(size);
            totalSizeInBytes += (uint) size;
        }

        foreach (int size in sizes)
        {
            Gl.EnableVertexAttribArray(location);
            Gl.VertexAttribPointer(location, size / 4, VertexAttribPointerType.Float, false, totalSizeInBytes, (void*) offset);
            offset += size;
            location += 1;
        }
    }
}