namespace Cubic.Graphics;

/// <summary>
/// Represents different graphics APIs Cubic supports.
/// </summary>
public enum GraphicsApi
{
    /// <summary>
    /// Cubic will automatically decide which platform to use based on the current platform (OS) and graphics
    /// specifications.
    /// </summary>
    Default,

    /// <summary>
    /// OpenGL 3.3
    /// </summary>
    OpenGL33,
    
    /// <summary>
    /// OpenGL ES 2.0
    /// </summary>
    GLES20
}