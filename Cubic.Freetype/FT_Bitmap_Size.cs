using System.Runtime.InteropServices;

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Bitmap_Size
{
    public short Height;
    public short Width;

    public int Size;

    public int Xppem;
    public int Yppem;
}