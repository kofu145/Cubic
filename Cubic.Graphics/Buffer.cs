using System;

namespace Cubic.Graphics;

/// <summary>
/// A native buffer used for graphics processing. Can be used for vertex buffers, index buffers, uniform buffers, etc.
/// </summary>
public abstract class Buffer : IDisposable
{
    /// <summary>
    /// Will return <see langword="true" /> when this buffer has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }

    /// <summary>
    /// Get the size in bytes of this buffer.
    /// </summary>
    public abstract uint Size { get; protected set; }
    
    /// <summary>
    /// Update this buffer with the given data array.
    /// </summary>
    /// <param name="offset">The offset, if any, in the buffer that this data should be sent to.</param>
    /// <param name="data">The data itself.</param>
    /// <typeparam name="T">Any unmanaged type that can be used for data uploading. Typical values include <see cref="float"/> and <see cref="ushort"/></typeparam>
    public abstract void Update<T>(int offset, T[] data) where T : unmanaged;

    /// <summary>
    /// Update this buffer with the given data pointer.
    /// </summary>
    /// <param name="offset">The offset, if any, in the buffer that this data should be sent to.</param>
    /// <param name="dataLength">The length of the data.</param>
    /// <param name="data">The pointer to the data.</param>
    /// <typeparam name="T">Any unmanaged type that can be used for data uploading. Typical values include <see cref="float"/> and <see cref="ushort"/></typeparam>
    public abstract void Update<T>(int offset, uint dataLength, IntPtr data) where T : unmanaged;

    /// <summary>
    /// Resize the buffer with a new size in bytes.
    /// </summary>
    /// <param name="size">The size in bytes to resize this buffer.</param>
    public abstract void Resize(uint size);
    
    /// <summary>
    /// Dispose of this buffer.
    /// </summary>
    public abstract void Dispose();
}