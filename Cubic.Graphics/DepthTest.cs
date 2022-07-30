namespace Cubic.Graphics;

/// <summary>
/// Represents various depth test options.
/// </summary>
public enum DepthTest
{
    /// <summary>
    /// Disable depth testing.
    /// </summary>
    Disable,
    Always,
    Equal,
    GreaterEqual,
    Greater,
    LessEqual,
    Less,
    NotEqual
}