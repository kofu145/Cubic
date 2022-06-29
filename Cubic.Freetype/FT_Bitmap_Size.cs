using System.Runtime.InteropServices;

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Bitmap_Size
{
    public short Height;
    public short Width;

    public long Size;

    public long Xppem;
    public long Yppem;
}