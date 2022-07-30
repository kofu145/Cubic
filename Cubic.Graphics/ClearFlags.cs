using System;

namespace Cubic.Graphics;

/// <summary>
/// Clear flags that can be set when clearing the back buffer, render targets, etc.
/// This has been marked with <b>[Flags]</b>, so use | to combine multiple.
/// <code>
/// GraphicsDevice.Clear(Color.Black, ClearFlags.Color | ClearFlags.Depth);
/// </code>
/// </summary>
[Flags]
public enum ClearFlags
{
    /// <summary>
    /// Clear the color buffer (aka the main screen buffer).
    /// </summary>
    Color,
    
    /// <summary>
    /// Clear the depth buffer.
    /// </summary>
    Depth,
    
    /// <summary>
    /// Clear the stencil buffer.
    /// </summary>
    Stencil
}