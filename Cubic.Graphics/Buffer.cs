using System;

namespace Cubic.Graphics;

public abstract class Buffer : IDisposable
{
    public abstract void Update<T>(int offset, T[] data) where T : unmanaged;
    
    public abstract void Dispose();
}