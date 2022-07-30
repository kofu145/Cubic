using System;

namespace Cubic.Graphics;

public abstract class Texture : IDisposable
{
    /// <summary>
    /// Will return <see langword="true" /> when this texture has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// Get or set the texture sample that should be used when rendering this texture.
    /// </summary>
    public abstract TextureSample Sample { get; set; }
    
    /// <summary>
    /// The texture usage of this texture.
    /// </summary>
    public abstract TextureUsage Usage { get; protected set; }
    
    /// <summary>
    /// The wrap mode that should be used for this texture.
    /// </summary>
    public abstract TextureWrap Wrap { get; set; }
    
    /// <summary>
    /// The pixel format of this texture.
    /// </summary>
    public abstract PixelFormat Format { get; }
    
    /// <summary>
    /// The anisotropic level of this texture. If mipmaps are enabled or have been generated already, changing this
    /// value will automatically regenerate the mipmaps.
    /// </summary>
    public abstract uint AnisotropicLevel { get; set; }
    
    /// <summary>
    /// Update this texture with the given data.
    /// </summary>
    /// <param name="x">The x-offset.</param>
    /// <param name="y">The y-offset.</param>
    /// <param name="width">The width of the data.</param>
    /// <param name="height">The height of the data.</param>
    /// <param name="data">The data itself.</param>
    /// <typeparam name="T">Any unmanaged type.</typeparam>
    public abstract void Update<T>(int x, int y, uint width, uint height, T[] data) where T : unmanaged;

    /// <summary>
    /// Update this texture with the given data, for cubemaps.
    /// </summary>
    /// <param name="x">The x-offset.</param>
    /// <param name="y">The y-offset.</param>
    /// <param name="width">The width of the data.</param>
    /// <param name="height">The height of the data.</param>
    /// <param name="data">The data itself.</param>
    /// <param name="position">The position on the cubemap of the data.</param>
    /// <typeparam name="T">Any unmanaged type.</typeparam>
    public abstract void Update<T>(int x, int y, uint width, uint height, T[] data,
        CubemapPosition position) where T : unmanaged;
    
    /// <summary>
    /// Update this texture with the given data.
    /// </summary>
    /// <param name="x">The x-offset.</param>
    /// <param name="y">The y-offset.</param>
    /// <param name="width">The width of the data.</param>
    /// <param name="height">The height of the data.</param>
    /// <param name="data">The data itself.</param>
    public abstract void Update(int x, int y, uint width, uint height, IntPtr data);

    /// <summary>
    /// Manually generate mipmaps for this texture. If mipmapping is not enabled, this will enable it.
    /// </summary>
    public abstract void GenerateMipmaps();
    
    /// <summary>
    /// Dispose of this texture.
    /// </summary>
    public abstract void Dispose();
}