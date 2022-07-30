using System;

namespace Cubic.Graphics;

/// <summary>
/// Represents a framebuffer/render target that can be rendered to.
/// </summary>
public abstract class Framebuffer : IDisposable
{
    /// <summary>
    /// Will return <see langword="true" /> when this framebuffer has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// Attach a <see cref="Texture"/> to this framebuffer. In order to use this texture as an attachment, it <b>must</b>
    /// be marked with <see cref="TextureUsage.Framebuffer"/>.
    /// </summary>
    /// <param name="texture">The texture to attach.</param>
    /// <param name="colorAttachment">The color attachment slot to use, if any (note this will only work if you attach a
    /// texture with an RGB or RGBA format.</param>
    public abstract void AttachTexture(Texture texture, int colorAttachment = 0);
    
    /// <summary>
    /// Dispose this framebuffer.
    /// </summary>
    public abstract void Dispose();
}