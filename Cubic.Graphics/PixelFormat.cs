namespace Cubic.Graphics;

/// <summary>
/// Represents some available pixel formats.
/// </summary>
public enum PixelFormat
{
    /// <summary>
    /// Red, Green, and Blue, each with 8 bits.
    /// </summary>
    RGB8,
    
    /// <summary>
    /// Red, Green, Blue, and Alpha, each with 8 bits.
    /// </summary>
    RGBA8,
    
    /// <summary>
    /// Blue, Red, Green, and Alpha, each with 8 bits.
    /// </summary>
    BRGA8,
    
    /// <summary>
    /// Use 24 bits for the depth buffer, and 8 bits for the stencil buffer.
    /// </summary>
    Depth24Stencil8
}