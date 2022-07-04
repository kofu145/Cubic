namespace Cubic.Graphics;

public struct ShaderLayout
{
    public string Name;
    public readonly int Size;
    public readonly AttribType Type;
    public readonly bool Normalize;

    public ShaderLayout(string name, int size, AttribType type, bool normalize = false)
    {
        Name = name;
        Size = size;
        Type = type;
        Normalize = normalize;
    }
}