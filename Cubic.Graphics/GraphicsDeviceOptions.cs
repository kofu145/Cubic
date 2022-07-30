namespace Cubic.Graphics;

/// <summary>
/// Represents a set of configurable options for the graphics device.
/// </summary>
public abstract class GraphicsDeviceOptions
{
    /// <summary>
    /// Set what depth test will be used. Set to <see cref="DepthTest.Disable"/> to disable depth testing.
    /// </summary>
    public abstract DepthTest DepthTest { get; set; }
    
    /// <summary>
    /// Enable/disable the scissor rectangle.
    /// </summary>
    public abstract bool EnableScissor { get; set; }
    
    /// <summary>
    /// Enable/disable the depth mask.
    /// </summary>
    public abstract bool DepthMask { get; set; }
    
    /// <summary>
    /// Set which face will be culled. Set to <see cref="CullFace.None"/> to disable culling.
    /// </summary>
    public abstract CullFace CullFace { get; set; }
    
    /// <summary>
    /// Set which direction the front face is.
    /// </summary>
    public abstract CullDirection CullDirection { get; set; }
}