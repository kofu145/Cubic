using System;

namespace Cubic.Graphics;

public abstract class Framebuffer : IDisposable
{
    public abstract void AttachTexture(Texture texture, int colorAttachment = 0);
    
    public abstract void Dispose();
}