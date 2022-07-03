using System;

namespace Cubic.Graphics;

public abstract class Texture : IDisposable
{
    public abstract TextureSample Sample { get; set; }
    
    public abstract TextureUsage Usage { get; set; }
    
    public abstract TextureWrap Wrap { get; set; }
    
    public abstract uint AnisotropicLevel { get; set; }
    
    public abstract void Update<T>(int x, int y, uint width, uint height, T[] data) where T : unmanaged;

    public abstract void Update<T>(int x, int y, uint width, uint height, T[] data,
        CubemapPosition position) where T : unmanaged;
    
    public abstract void Update(int x, int y, uint width, uint height, IntPtr data);

    public abstract void GenerateMipmaps();
    
    public abstract void Dispose();
}