using System;
using System.Runtime.CompilerServices;
using Silk.NET.OpenGL;
using static Cubic.Graphics.Platforms.OpenGL33.OpenGL33GraphicsDevice;

namespace Cubic.Graphics.Platforms.OpenGL33;

public class OpenGL33Buffer : Buffer
{
    public readonly uint Handle;
    public readonly BufferTargetARB Target;
    public Type Type;
    
    internal unsafe OpenGL33Buffer(BufferType type, uint size)
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
    }

    public override unsafe void Update<T>(int offset, T[] data)
    {
        Gl.BindBuffer(Target, Handle);
        fixed (void* dat = data)
            Gl.BufferSubData(Target, offset, (nuint) (data.Length * Unsafe.SizeOf<T>()), dat);
        Type = data.GetType().GetElementType();
    }

    public override void Dispose()
    {
        Gl.DeleteBuffer(Handle);
    }
}