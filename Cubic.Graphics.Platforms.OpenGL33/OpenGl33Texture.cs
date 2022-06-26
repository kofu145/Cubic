using System;
using Silk.NET.OpenGL;
using static Cubic.Graphics.Platforms.OpenGL33.OpenGl33GraphicsDevice;

namespace Cubic.Graphics.Platforms.OpenGL33;

public class OpenGl33Texture : Texture
{
    public uint Handle;
    public Silk.NET.OpenGL.PixelFormat Format;
    public TextureUsage TextureUsage;
    public bool Mipmap;
    private TextureSample _sample;
    private TextureWrap _wrap;
    public TextureTarget Target;

    internal unsafe OpenGl33Texture(uint width, uint height, PixelFormat format, TextureSample sample, bool mipmap, TextureUsage usage, TextureWrap wrap)
    {
        Mipmap = mipmap;
        TextureUsage = usage;
        Handle = Gl.GenTexture();
        Target = usage switch
        {
            TextureUsage.Texture => TextureTarget.Texture2D,
            TextureUsage.Framebuffer => TextureTarget.Texture2D,
            TextureUsage.Cubemap => TextureTarget.TextureCubeMap,
            _ => throw new ArgumentOutOfRangeException(nameof(usage), usage, null)
        };
        Gl.BindTexture(Target, Handle);
        Format = format switch
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
                        Format, PixelType.UnsignedByte, null);
                break;
            default:
                Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, width, height, 0, Format, PixelType.UnsignedByte,
                    null);
                break;
        }

        _wrap = wrap;

        TextureWrapMode mode = wrap switch
        {
            TextureWrap.Repeat => TextureWrapMode.Repeat,
            TextureWrap.Clamp => TextureWrapMode.ClampToEdge,
            _ => throw new ArgumentOutOfRangeException(nameof(wrap), wrap, null)
        };
        
        Gl.TexParameter(Target, TextureParameterName.TextureWrapS, (int) mode);
        Gl.TexParameter(Target, TextureParameterName.TextureWrapT, (int) mode);
        if (usage == TextureUsage.Framebuffer)
            Gl.TexParameter(Target, GLEnum.TextureWrapR, (int) mode);
        Gl.TexParameter(Target, GLEnum.TextureMinFilter,
            sample == TextureSample.Linear ? (int) TextureMinFilter.Linear : (int) TextureMinFilter.Nearest);
        Gl.TexParameter(Target, GLEnum.TextureMagFilter,
            sample == TextureSample.Linear ? (int) TextureMagFilter.Linear : (int) TextureMagFilter.Nearest);
    }

    public override TextureSample Sample
    {
        get => _sample;
        set
        {
            _sample = value;
            Gl.BindTexture(TextureTarget.Texture2D, Handle);
            Gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureMinFilter,
                value == TextureSample.Linear ? (int) TextureMinFilter.Linear : (int) TextureMinFilter.Nearest);
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
        get => TextureWrap.Clamp;
        set
        {
            TextureWrapMode mode = value switch
            {
                TextureWrap.Repeat => TextureWrapMode.Repeat,
                TextureWrap.Clamp => TextureWrapMode.ClampToEdge,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
            
            Gl.TexParameter(Target, TextureParameterName.TextureWrapS, (int) mode);
            Gl.TexParameter(Target, TextureParameterName.TextureWrapT, (int) mode);
            if (TextureUsage == TextureUsage.Framebuffer)
                Gl.TexParameter(Target, GLEnum.TextureWrapR, (int) mode);
        }
    }

    public override unsafe void Update<T>(int x, int y, uint width, uint height, T[] data)
    {
        Gl.BindTexture(TextureTarget.Texture2D, Handle);
        fixed (void* dat = data)
            Gl.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, width, height, Format, PixelType.UnsignedByte, dat);
        
        if (Mipmap)
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
            Gl.TexSubImage2D(target, 0, x, y, width, height, Format, PixelType.UnsignedByte, dat);
        
        if (Mipmap)
            Gl.GenerateMipmap(TextureTarget.TextureCubeMap);
    }

    public override unsafe void Update(int x, int y, uint width, uint height, IntPtr data)
    {
        Gl.BindTexture(TextureTarget.Texture2D, Handle);
        Gl.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, width, height, Format, PixelType.UnsignedByte, data.ToPointer());
        
        Gl.GenerateMipmap(TextureTarget.Texture2D);
    }

    public override void GenerateMipmaps()
    {
        Gl.BindTexture(TextureTarget.Texture2D, Handle);
        Gl.GenerateMipmap(TextureTarget.Texture2D);
    }

    public override void Dispose()
    {
        Gl.DeleteTexture(Handle);
    }
}