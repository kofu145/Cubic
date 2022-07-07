namespace Cubic.Extensions.Imgui;

public struct ImGuiFont
{
    public string Name;
    public string Path;
    public uint Size;

    public ImGuiFont(string name, string path, uint size)
    {
        Name = name;
        Path = path;
        Size = size;
    }
}