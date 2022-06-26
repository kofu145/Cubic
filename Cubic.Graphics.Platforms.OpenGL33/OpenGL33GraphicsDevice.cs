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

    public static GL Gl;
    private uint _vao;
    private Rectangle _viewport;
    private Rectangle _scissor;
    private DepthTest _depthTest;
    private bool _depthEnabled;
    private bool _depthMask;
    private CullFace _face;
    private CullDirection _dir;

    private IGLContext _context;
    
    public OpenGL33GraphicsDevice(IGLContext context)
    {
        Gl = GL.GetApi(context);
        _vao = Gl.GenVertexArray();
        Gl.BindVertexArray(_vao);
        _context = context;
        
        // TODO: This
        Gl.Enable(EnableCap.Blend);
        Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    public override bool DepthMask
    {
        get => _depthMask;
        set
        {
            _depthMask = value;
            Gl.DepthMask(value);
        }
    }

    public override CullFace CullFace
    {
        get => _face;
        set
        {
            _face = value;
            if (value == CullFace.None)
                Gl.Disable(EnableCap.CullFace);
            else
            {
                Gl.Enable(EnableCap.CullFace);
                Gl.CullFace(value == CullFace.Front ? CullFaceMode.Front : CullFaceMode.Back);
            }
        }
    }

    public override CullDirection CullDirection
    {
        get => _dir;
        set
        {
            _dir = value;
            Gl.FrontFace(value == CullDirection.Clockwise ? FrontFaceDirection.CW : FrontFaceDirection.Ccw);
        }
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
    
    public override DepthTest DepthTest
    {
        get => _depthTest;
        set
        {
            _depthTest = value;
            if (value == DepthTest.Disable && _depthEnabled)
            {
                _depthEnabled = false;
                Gl.Disable(EnableCap.DepthTest);
            }
            else if (!_depthEnabled)
            {
                _depthEnabled = true;
                Gl.Enable(EnableCap.DepthTest);
                Gl.DepthFunc(value switch
                {
                    DepthTest.Disable => DepthFunction.Never,
                    DepthTest.Always => DepthFunction.Always,
                    DepthTest.Equal => DepthFunction.Equal,
                    DepthTest.GreaterEqual => DepthFunction.Gequal,
                    DepthTest.Greater => DepthFunction.Greater,
                    DepthTest.LessEqual => DepthFunction.Lequal,
                    DepthTest.Less => DepthFunction.Less,
                    DepthTest.NotEqual => DepthFunction.Notequal,
                    _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
                });
            }
        }
    }

    public override bool EnableScissor
    {
        get => Gl.IsEnabled(EnableCap.ScissorTest);
        set
        {
            if (value)
                Gl.Enable(EnableCap.ScissorTest);
            else
                Gl.Disable(EnableCap.ScissorTest);
        }
    }

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
        buf.Type = data.GetType().GetElementType();
    }

    public override unsafe Texture CreateTexture(uint width, uint height, PixelFormat format, TextureSample sample = TextureSample.Linear, bool mipmap = true, TextureUsage usage = TextureUsage.Texture)
    {
        uint handle = Gl.GenTexture();
        TextureTarget target = usage switch
        {
            TextureUsage.Texture => TextureTarget.Texture2D,
            TextureUsage.Framebuffer => TextureTarget.Texture2D,
            TextureUsage.Cubemap => TextureTarget.TextureCubeMap,
            _ => throw new ArgumentOutOfRangeException(nameof(usage), usage, null)
        };
        Gl.BindTexture(target, handle);
        Silk.NET.OpenGL.PixelFormat fmt = format switch
        {
            PixelFormat.RGB => Silk.NET.OpenGL.PixelFormat.Rgb,
            PixelFormat.RGBA => Silk.NET.OpenGL.PixelFormat.Rgba,
            PixelFormat.BRGA => Silk.NET.OpenGL.PixelFormat.Bgra,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };

        switch (usage)
        {
            
            case TextureUsage.Cubemap:
                for (int i = 0; i < 6; i++)
                    Gl.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, InternalFormat.Rgba, width, height, 0,
                        fmt, PixelType.UnsignedByte, null);
                break;
            default:
                Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, width, height, 0, fmt, PixelType.UnsignedByte,
                    null);
                break;
        }


        Gl.TexParameter(target, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
        Gl.TexParameter(target, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);
        if (usage == TextureUsage.Framebuffer)
            Gl.TexParameter(target, GLEnum.TextureWrapR, (int) TextureWrapMode.Repeat);
        Gl.TexParameter(target, GLEnum.TextureMinFilter,
            sample == TextureSample.Linear ? (int) TextureMinFilter.Linear : (int) TextureMinFilter.Nearest);
        Gl.TexParameter(target, GLEnum.TextureMagFilter,
            sample == TextureSample.Linear ? (int) TextureMagFilter.Linear : (int) TextureMagFilter.Nearest);
        
        return new OpenGL33Texture(handle, fmt, usage, mipmap);
    }

    public override Framebuffer CreateFramebuffer()
    {
        return new OpenGL33Framebuffer(Gl.CreateFramebuffer());
    }

    public override void AttachTextureToFramebuffer(Framebuffer framebuffer, Texture texture, int colorAttachment = 0)
    {
        OpenGL33Texture tex = (OpenGL33Texture) texture;
        OpenGL33Framebuffer buf = (OpenGL33Framebuffer) framebuffer;
        switch (tex.Usage)
        {
            case TextureUsage.Texture:
                throw new GraphicsException("Regular Textures cannot be used as a framebuffer attachment.");
            case TextureUsage.Framebuffer:
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, buf.Handle);
                Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                    (FramebufferAttachment) (int) FramebufferAttachment.ColorAttachment0 + colorAttachment,
                    TextureTarget.Texture2D, tex.Handle, 0);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override unsafe void UpdateTexture<T>(Texture texture, int x, int y, uint width, uint height, T[] data)
    {
        OpenGL33Texture tex = (OpenGL33Texture) texture;
        
        Gl.BindTexture(TextureTarget.Texture2D, tex.Handle);
        fixed (void* dat = data)
            Gl.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, width, height, tex.Format, PixelType.UnsignedByte, dat);
        
        if (tex.Mipmap)
            Gl.GenerateMipmap(TextureTarget.Texture2D);
    }

    public override unsafe void UpdateTexture<T>(Texture texture, int x, int y, uint width, uint height, T[] data, CubemapPosition position)
    {
        OpenGL33Texture tex = (OpenGL33Texture) texture;

        TextureTarget target = position switch
        {
            CubemapPosition.PositiveX => TextureTarget.TextureCubeMapPositiveX,
            CubemapPosition.NegativeX => TextureTarget.TextureCubeMapNegativeX,
            CubemapPosition.PositiveY => TextureTarget.TextureCubeMapPositiveY,
            CubemapPosition.NegativeY => TextureTarget.TextureCubeMapNegativeY,
            CubemapPosition.PositiveZ => TextureTarget.TextureCubeMapPositiveZ,
            CubemapPosition.NegativeZ => TextureTarget.TextureCubeMapNegativeZ,
            _ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
        };
        
        Gl.BindTexture(TextureTarget.TextureCubeMap, tex.Handle);
        fixed (void* dat = data)
            Gl.TexSubImage2D(target, 0, x, y, width, height, tex.Format, PixelType.UnsignedByte, dat);
        
        if (tex.Mipmap)
            Gl.GenerateMipmap(TextureTarget.TextureCubeMap);
    }

    public override unsafe void UpdateTexture(Texture texture, int x, int y, uint width, uint height, IntPtr data)
    {
        OpenGL33Texture tex = (OpenGL33Texture) texture;
        
        Gl.BindTexture(TextureTarget.Texture2D, tex.Handle);
        Gl.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, width, height, tex.Format, PixelType.UnsignedByte, data.ToPointer());
        
        Gl.GenerateMipmap(TextureTarget.Texture2D);
    }

    public override void SetTextureSample(Texture texture, TextureSample sample)
    {
        OpenGL33Texture tex = (OpenGL33Texture) texture;
        
        Gl.BindTexture(TextureTarget.Texture2D, tex.Handle);
        Gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureMinFilter,
            sample == TextureSample.Linear ? (int) TextureMinFilter.Linear : (int) TextureMinFilter.Nearest);
        Gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureMagFilter,
            sample == TextureSample.Linear ? (int) TextureMagFilter.Linear : (int) TextureMagFilter.Nearest);
    }

    public override void SetTextureUsage(Texture texture, TextureUsage usage)
    {
        ((OpenGL33Texture) texture).Usage = usage;
    }

    public override void GenerateTextureMipmaps(Texture texture)
    {
        OpenGL33Texture tex = (OpenGL33Texture) texture;
        
        Gl.BindTexture(TextureTarget.Texture2D, tex.Handle);
        Gl.GenerateMipmap(TextureTarget.Texture2D);
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

        Dictionary<string, int> uLocations = new Dictionary<string, int>();
        Gl.GetProgram(handle, ProgramPropertyARB.ActiveUniforms, out int numUniforms);
        for (uint i = 0; i < numUniforms; i++)
        {
            string name = Gl.GetActiveUniform(handle, i, out _, out _);
            int location = Gl.GetUniformLocation(handle, name);
            uLocations.Add(name, location);
        }
        
        return new OpenGl33ShaderProgram(handle, uLocations);
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
        Gl.ClearColor(color.X, color.Y, color.Z, color.W);
        Gl.Clear((uint) ClearBufferMask.ColorBufferBit | (uint) ClearBufferMask.DepthBufferBit);
    }

    public override void SetUniform(ShaderProgram program, string uniformName, bool value)
    {
        OpenGl33ShaderProgram prg = (OpenGl33ShaderProgram) program;
        Gl.UseProgram(prg.Handle);
        Gl.Uniform1(prg.UniformLocations[uniformName], value ? 1 : 0);
    }

    public override void SetUniform(ShaderProgram program, string uniformName, int value)
    {
        OpenGl33ShaderProgram prg = (OpenGl33ShaderProgram) program;
        Gl.UseProgram(prg.Handle);
        Gl.Uniform1(prg.UniformLocations[uniformName], value);
    }

    public override void SetUniform(ShaderProgram program, string uniformName, float value)
    {
        OpenGl33ShaderProgram prg = (OpenGl33ShaderProgram) program;
        Gl.UseProgram(prg.Handle);
        Gl.Uniform1(prg.UniformLocations[uniformName], value);
    }

    public override void SetUniform(ShaderProgram program, string uniformName, Vector2 value)
    {
        OpenGl33ShaderProgram prg = (OpenGl33ShaderProgram) program;
        Gl.UseProgram(prg.Handle);
        Gl.Uniform2(prg.UniformLocations[uniformName], ref value);
    }

    public override void SetUniform(ShaderProgram program, string uniformName, Vector3 value)
    {
        OpenGl33ShaderProgram prg = (OpenGl33ShaderProgram) program;
        Gl.UseProgram(prg.Handle);
        Gl.Uniform3(prg.UniformLocations[uniformName], ref value);
    }

    public override void SetUniform(ShaderProgram program, string uniformName, Vector4 value)
    {
        OpenGl33ShaderProgram prg = (OpenGl33ShaderProgram) program;
        Gl.UseProgram(prg.Handle);
        Gl.Uniform4(prg.UniformLocations[uniformName], ref value);
    }

    public override void SetUniform(ShaderProgram program, string uniformName, Color color)
    {
        OpenGl33ShaderProgram prg = (OpenGl33ShaderProgram) program;
        Gl.UseProgram(prg.Handle);
        Vector4 normalized = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        Gl.Uniform4(prg.UniformLocations[uniformName], ref normalized);
    }

    public override unsafe void SetUniform(ShaderProgram program, string uniformName, Matrix4x4 matrix, bool transpose = true)
    {
        OpenGl33ShaderProgram prg = (OpenGl33ShaderProgram) program;
        Gl.UseProgram(prg.Handle);
        Gl.UniformMatrix4(prg.UniformLocations[uniformName], 1, transpose, (float*) &matrix);
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

    public override void SetTexture(uint slot, Texture texture)
    {
        OpenGL33Texture tex = (OpenGL33Texture) texture;
        TextureTarget target = tex.Usage switch
        {
            TextureUsage.Texture => TextureTarget.Texture2D,
            TextureUsage.Framebuffer => TextureTarget.Texture2D,
            TextureUsage.Cubemap => TextureTarget.TextureCubeMap,
            _ => throw new ArgumentOutOfRangeException(nameof(tex.Usage), tex.Usage, null)
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