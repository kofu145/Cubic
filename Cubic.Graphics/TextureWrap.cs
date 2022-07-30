namespace Cubic.Graphics;

/// <summary>
/// Represents different wrapping types for textures.
/// </summary>
public enum TextureWrap
{
    /// <summary>
    /// This texture will repeat outside of its bounds.
    /// </summary>
    Repeat,
    
    /// <summary>
    /// This texture will clamp to edge outside of its bounds.
    /// </summary>
    ClampToEdge,
    
    /// <summary>
    /// This texture will clamp to the border outside its bounds.
    /// </summary>
    ClampToBorder
}