using System;
using System.Drawing;
using System.Numerics;
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
    private Color _clampColor;

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
        protected set => TextureUsage = value;
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
                TextureWrap.ClampToEdge => TextureWrapMode.ClampToEdge,
                TextureWrap.ClampToBorder => TextureWrapMode.ClampToBorder,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
            
            Gl.TexParameter(_target, TextureParameterName.TextureWrapS, (int) mode);
            Gl.TexParameter(_target, TextureParameterName.TextureWrapT, (int) mode);
            if (TextureUsage == TextureUsage.Framebuffer)
                Gl.TexParameter(_target, GLEnum.TextureWrapR, (int) mode);
        }
    }
    
    public override unsafe Color BorderColor
    {
        get => _clampColor;
        set
        {
            _clampColor = value;
            Vector4 clamp = new Vector4(value.R / 255f, value.G / 255f, value.B / 255f, value.A / 255f);
            Gl.BindTexture(TextureTarget.Texture2D, Handle);
            fixed (float* border = new float[] { clamp.X, clamp.Y, clamp.Z, clamp.W })
                Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, border);
        }
    }

    public override PixelFormat Format { get; }

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
        Format = format;
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
            PixelFormat.RGB8 => Silk.NET.OpenGLES.PixelFormat.Rgb,
            PixelFormat.RGBA8 => Silk.NET.OpenGLES.PixelFormat.Rgba,
            PixelFormat.BRGA8 => Silk.NET.OpenGLES.PixelFormat.Bgra,
            PixelFormat.Depth24Stencil8 => Silk.NET.OpenGLES.PixelFormat.DepthStencil,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };

        switch (usage)
        {
            
            case TextureUsage.Cubemap:
                for (int i = 0; i < 6; i++)
                    Gl.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, InternalFormat.Rgba, width, height, 0,
                        _format, PixelType.UnsignedByte, null);
                break;
            default:
                InternalFormat iFormat = InternalFormat.Rgba;
                PixelType pType = PixelType.UnsignedByte;
                if (format == PixelFormat.Depth24Stencil8)
                {
                    iFormat = InternalFormat.Depth24Stencil8;
                    pType = (PixelType) GLEnum.UnsignedInt248;
                }
                
                Gl.TexImage2D(TextureTarget.Texture2D, 0, iFormat, width, height, 0, _format, pType,
                    null);
                break;
        }

        _wrap = wrap;
        _anisotropicLevel = anisotropicLevel;

        TextureWrapMode mode = wrap switch
        {
            TextureWrap.Repeat => TextureWrapMode.Repeat,
            TextureWrap.ClampToEdge => TextureWrapMode.ClampToEdge,
            TextureWrap.ClampToBorder => TextureWrapMode.ClampToBorder,
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