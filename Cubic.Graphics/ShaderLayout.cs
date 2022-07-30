namespace Cubic.Graphics;

/// <summary>
/// Defines a shader layout that is used when setting the vertex buffer.
/// </summary>
public struct ShaderLayout
{
    /// <summary>
    /// The name of the attribute.
    /// </summary>
    public readonly string Name;
    
    /// <summary>
    /// The size of the attribute.
    /// </summary>
    public readonly int Size;
    
    /// <summary>
    /// The type of this attribute.
    /// </summary>
    public readonly AttribType Type;
    
    /// <summary>
    /// Whether or not to normalize the values that are passed in.
    /// </summary>
    public readonly bool Normalize;

    /// <summary>
    /// Create a new shader layout.
    /// </summary>
    /// <param name="name">The name of the attribute.</param>
    /// <param name="size">The size of the attribute.</param>
    /// <param name="type">The type of this attribute.</param>
    /// <param name="normalize">Whether or not to normalize the values that are passed in.</param>
    public ShaderLayout(string name, int size, AttribType type, bool normalize = false)
    {
        Name = name;
        Size = size;
        Type = type;
        Normalize = normalize;
    }
}