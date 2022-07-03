using System.Runtime.InteropServices;
#if WINDOWS
using Long = System.Int32;
#elif LINUX
using Long = System.Int64;
#endif

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Bitmap_Size
{
    public short Height;
    public short Width;

    public Long Size;

    public Long Xppem;
    public Long Yppem;
}