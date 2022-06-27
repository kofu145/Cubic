namespace Cubic.Graphics;

public struct ShaderLayout
{
    public readonly int Size;
    public readonly AttribType Type;
    public readonly bool Normalize;

    public ShaderLayout(int size, AttribType type, bool normalize = false)
    {
        Size = size;
        Type = type;
        Normalize = normalize;
    }
}