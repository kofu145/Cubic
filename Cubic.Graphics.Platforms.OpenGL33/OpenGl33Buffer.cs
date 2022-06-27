using System;
using System.Runtime.CompilerServices;
using Silk.NET.OpenGL;
using static Cubic.Graphics.Platforms.OpenGL33.OpenGl33GraphicsDevice;

namespace Cubic.Graphics.Platforms.OpenGL33;

public class OpenGl33Buffer : Buffer
{
    public readonly uint Handle;
    public readonly BufferTargetARB Target;
    public Type Type;
    
    internal unsafe OpenGl33Buffer(BufferType type, uint size)
    {
        Handle = Gl.GenBuffer();
        Target = type switch
        {
            BufferType.VertexBuffer => BufferTargetARB.ArrayBuffer,
            BufferType.IndexBuffer => BufferTargetARB.ElementArrayBuffer,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        Gl.BindBuffer(Target, Handle);
        Gl.BufferData(Target, size, null, BufferUsageARB.DynamicDraw);
        Size = size;
    }

    public override uint Size { get; protected set; }

    public override unsafe void Update<T>(int offset, T[] data)
    {
        Gl.BindBuffer(Target, Handle);
        fixed (void* dat = data)
            Gl.BufferSubData(Target, offset * Unsafe.SizeOf<T>(), (nuint) (data.Length * Unsafe.SizeOf<T>()), dat);
        Type = typeof(T);
    }

    public override unsafe void Update<T>(int offset, uint dataLength, IntPtr data)
    {
        Gl.BindBuffer(Target, Handle);
        Gl.BufferSubData(Target, offset * Unsafe.SizeOf<T>(), (nuint) (dataLength * Unsafe.SizeOf<T>()), data.ToPointer());
        Type = typeof(T);
    }

    public override unsafe void Resize(uint size)
    {
        Gl.BindBuffer(Target, Handle);
        Gl.BufferData(Target, size, null, BufferUsageARB.DynamicDraw);
        Size = size;
    }

    public override void Dispose()
    {
        Gl.DeleteBuffer(Handle);
    }
}