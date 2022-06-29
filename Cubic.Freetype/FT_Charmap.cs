using System.Runtime.InteropServices;

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Charmap
{
    public FT_Face Face;
    public FT_Encoding Encoding;
    public ushort PlatformID;
    public ushort EncodingID;
}