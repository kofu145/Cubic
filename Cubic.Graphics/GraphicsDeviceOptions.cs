namespace Cubic.Graphics;

public abstract class GraphicsDeviceOptions
{
    public abstract DepthTest DepthTest { get; set; }
    
    public abstract bool EnableScissor { get; set; }
    
    public abstract bool DepthMask { get; set; }
    
    public abstract CullFace CullFace { get; set; }
    
    public abstract CullDirection CullDirection { get; set; }
}