using System;

namespace Cubic.Graphics;

public abstract class Buffer : IDisposable
{
    public abstract bool IsDisposed { get; protected set; }

    public abstract uint Size { get; protected set; }
    
    public abstract void Update<T>(int offset, T[] data) where T : unmanaged;

    public abstract void Update<T>(int offset, uint dataLength, IntPtr data) where T : unmanaged;

    public abstract void Resize(uint size);
    
    public abstract void Dispose();
}