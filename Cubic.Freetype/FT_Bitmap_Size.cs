using System.Runtime.InteropServices;

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Bitmap_Size
{
    public short Height;
    public short Width;

    public CLong Size;

    public CLong Xppem;
    public CLong Yppem;
}