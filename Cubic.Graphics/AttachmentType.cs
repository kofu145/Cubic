namespace Cubic.Graphics;

/// <summary>
/// Represents an attachment type for a <see cref="ShaderAttachment"/>.
/// </summary>
public enum AttachmentType
{
    /// <summary>
    /// Vertex shader.
    /// </summary>
    Vertex,
    
    /// <summary>
    /// Fragment shader. (Equivalent to <see cref="Pixel"/>)
    /// </summary>
    Fragment,
    
    /// <summary>
    /// Pixel shader. (Equivalent to <see cref="Fragment"/>)
    /// </summary>
    Pixel = 1,
    
    /// <summary>
    /// Geometry shader.
    /// </summary>
    Geometry
}