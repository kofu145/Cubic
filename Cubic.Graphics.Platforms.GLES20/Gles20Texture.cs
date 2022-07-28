using System;
using System.Reflection.Metadata;
using Silk.NET.OpenGLES;
using static Cubic.Graphics.Platforms.GLES20.Gles20GraphicsDevice;

namespace Cubic.Graphics.Platforms.GLES20;

public class Gles20Texture : Texture
{
    public uint Handle;
    private Silk.NET.OpenGLES.PixelFormat _format;
    public TextureUsage TextureUsage;
    private bool _mipmap;
    private TextureSample _sample;
    private TextureWrap _wrap;
    private TextureTarget _target;
    private uint _anisotropicLevel;

    public override bool IsDisposed { get; protected set; }

    public override TextureSample Sample
    {
        get => _sample;
        set
        {
            _sample = value;
            Gl.BindTexture(TextureTarget.Texture2D, Handle);
            Gl.TexParameter(_target, GLEnum.TextureMinFilter,
                value == TextureSample.Linear
                    ?
                    _mipmap ? (int) TextureMinFilter.LinearMipmapLinear : (int) TextureMinFilter.Linear
                    : _mipmap
                        ? (int) TextureMinFilter.NearestMipmapNearest
                        : (int) TextureMinFilter.Nearest);
            Gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureMagFilter,
                value == TextureSample.Linear ? (int) TextureMagFilter.Linear : (int) TextureMagFilter.Nearest);
        }
    }

    public override TextureUsage Usage
    {
        get => TextureUsage;
        set => TextureUsage = value;
    }

    public override TextureWrap Wrap
    {
        get => _wrap;
        set
        {
            _wrap = value;
            TextureWrapMode mode = value switch
            {
                TextureWrap.Repeat => TextureWrapMode.Repeat,
                TextureWrap.Clamp => TextureWrapMode.ClampToEdge,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
            
            Gl.TexParameter(_target, TextureParameterName.TextureWrapS, (int) mode);
            Gl.TexParameter(_target, TextureParameterName.TextureWrapT, (int) mode);
            if (TextureUsage == TextureUsage.Framebuffer)
                Gl.TexParameter(_target, GLEnum.TextureWrapR, (int) mode);
        }
    }

    public override uint AnisotropicLevel
    {
        get => _anisotropicLevel;
        set
        {
            _anisotropicLevel = value;
            Gl.BindTexture(TextureTarget.Texture2D, Handle);
            Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxAnisotropy, value);
            if (_mipmap)
                Gl.GenerateMipmap(TextureTarget.Texture2D);
        }
    }
    
    internal unsafe Gles20Texture(uint width, uint height, PixelFormat format, TextureSample sample, bool mipmap, TextureUsage usage, TextureWrap wrap, uint anisotropicLevel)
    {
        _mipmap = mipmap;
        TextureUsage = usage;
        Handle = Gl.GenTexture();
        _target = usage switch
        {
            TextureUsage.Texture => TextureTarget.Texture2D,
            TextureUsage.Framebuffer => TextureTarget.Texture2D,
            TextureUsage.Cubemap => TextureTarget.TextureCubeMap,
            _ => throw new ArgumentOutOfRangeException(nameof(usage), usage, null)
        };
        Gl.BindTexture(_target, Handle);
        _format = format switch
        {
            PixelFormat.RGB => Silk.NET.OpenGLES.PixelFormat.Rgb,
            PixelFormat.RGBA => Silk.NET.OpenGLES.PixelFormat.Rgba,
            PixelFormat.BRGA => Silk.NET.OpenGLES.PixelFormat.Bgra,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };

        InternalFormat fmt = format switch
        {
            PixelFormat.RGB => Silk.NET.OpenGLES.InternalFormat.Rgb,
            PixelFormat.RGBA => Silk.NET.OpenGLES.InternalFormat.Rgba,
            PixelFormat.BRGA => throw new NotSupportedException("GLES 2.0 does not support BGRA textures."),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };

        switch (usage)
        {
            
            case TextureUsage.Cubemap:
                for (int i = 0; i < 6; i++)
                    Gl.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, fmt, width, height, 0,
                        _format, PixelType.UnsignedByte, null);
                break;
            default:
                Gl.TexImage2D(TextureTarget.Texture2D, 0, fmt, width, height, 0, _format, PixelType.UnsignedByte,
                    null);
                break;
        }

        _wrap = wrap;
        _anisotropicLevel = anisotropicLevel;

        TextureWrapMode mode = wrap switch
        {
            TextureWrap.Repeat => TextureWrapMode.Repeat,
            TextureWrap.Clamp => TextureWrapMode.ClampToEdge,
            _ => throw new ArgumentOutOfRangeException(nameof(wrap), wrap, null)
        };
        
        Gl.TexParameter(_target, TextureParameterName.TextureWrapS, (int) mode);
        Gl.TexParameter(_target, TextureParameterName.TextureWrapT, (int) mode);
        if (usage == TextureUsage.Framebuffer)
            Gl.TexParameter(_target, GLEnum.TextureWrapR, (int) mode);
        Gl.TexParameter(_target, GLEnum.TextureMinFilter,
            sample == TextureSample.Linear
                ?
                mipmap ? (int) TextureMinFilter.LinearMipmapLinear : (int) TextureMinFilter.Linear
                : mipmap
                    ? (int) TextureMinFilter.NearestMipmapNearest
                    : (int) TextureMinFilter.Nearest);
        Gl.TexParameter(_target, GLEnum.TextureMagFilter,
            sample == TextureSample.Linear ? (int) TextureMagFilter.Linear : (int) TextureMagFilter.Nearest);
    }
    
    public override unsafe void Update<T>(int x, int y, uint width, uint height, T[] data)
    {
        Gl.BindTexture(TextureTarget.Texture2D, Handle);
        fixed (void* dat = data)
            Gl.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, width, height, _format, PixelType.UnsignedByte, dat);
        
        Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxAnisotropy, _anisotropicLevel);
        if (_mipmap)
            Gl.GenerateMipmap(TextureTarget.Texture2D);
    }

    public override unsafe void Update<T>(int x, int y, uint width, uint height, T[] data, CubemapPosition position)
    {
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
        
        Gl.BindTexture(TextureTarget.TextureCubeMap, Handle);
        fixed (void* dat = data)
            Gl.TexSubImage2D(target, 0, x, y, width, height, _format, PixelType.UnsignedByte, dat);
        
        Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxAnisotropy, _anisotropicLevel);
        if (_mipmap)
            Gl.GenerateMipmap(TextureTarget.TextureCubeMap);
    }

    public override unsafe void Update(int x, int y, uint width, uint height, IntPtr data)
    {
        Gl.BindTexture(TextureTarget.Texture2D, Handle);
        Gl.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, width, height, _format, PixelType.UnsignedByte, data.ToPointer());
        
        Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxAnisotropy, _anisotropicLevel);
        if (_mipmap)
            Gl.GenerateMipmap(TextureTarget.Texture2D);
    }

    public override void GenerateMipmaps()
    {
        Gl.BindTexture(TextureTarget.Texture2D, Handle);
        Gl.GenerateMipmap(TextureTarget.Texture2D);
        _mipmap = true;
        Sample = _sample;
    }

    public override void Dispose()
    {
        if (IsDisposed) return;
        IsDisposed = true;
        Gl.DeleteTexture(Handle);
    }
}