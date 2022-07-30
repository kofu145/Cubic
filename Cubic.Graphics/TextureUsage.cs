namespace Cubic.Graphics;

/// <summary>
/// Represents different usecases for textures.
/// </summary>
public enum TextureUsage
{
    /// <summary>
    /// Use this texture as a renderable texture.
    /// </summary>
    Texture,
    
    /// <summary>
    /// Use this texture as a framebuffer attachment.
    /// </summary>
    Framebuffer,
    
    /// <summary>
    /// Use this texture as a cubemap.
    /// </summary>
    Cubemap
}