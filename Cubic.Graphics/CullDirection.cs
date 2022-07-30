namespace Cubic.Graphics;

/// <summary>
/// Represents different cull directions for face culling.
/// </summary>
public enum CullDirection
{
    /// <summary>
    /// Cull clockwise.
    /// </summary>
    Clockwise,
    
    /// <summary>
    /// Cull counter clockwise (equivalent to <see cref="AntiClockwise"/>)
    /// </summary>
    CounterClockwise,
    
    /// <summary>
    /// Cull anti clockwise (equivalent to <see cref="CounterClockwise"/>)
    /// </summary>
    AntiClockwise = 1
}