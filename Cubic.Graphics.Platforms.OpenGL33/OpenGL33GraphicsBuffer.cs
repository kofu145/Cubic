using Silk.NET.OpenGL;
using static Cubic.Graphics.Platforms.OpenGL33.OpenGL33GraphicsDevice;

namespace Cubic.Graphics.Platforms.OpenGL33;

public class OpenGL33GraphicsBuffer : GraphicsBuffer
{
    public readonly uint Handle;
    public readonly BufferTargetARB Target;
    
    public OpenGL33GraphicsBuffer(uint id, BufferTargetARB target)
    {
        Handle = id;
        Target = target;
    }
    
    public override void Dispose()
    {
        Gl.DeleteBuffer(Handle);
    }
}