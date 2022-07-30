namespace Cubic.Graphics;

/// <summary>
/// Represents a list of possible faces that can be culled. <see cref="None"/> disables culling.
/// </summary>
public enum CullFace
{
    /// <summary>
    /// Cull no faces.
    /// </summary>
    None,
    
    /// <summary>
    /// Cull front faces.
    /// </summary>
    Front,
    
    /// <summary>
    /// Cull back faces.
    /// </summary>
    Back
}