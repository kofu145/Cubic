using System.Runtime.InteropServices;
#if WINDOWS
using Long = System.Int32;
#elif LINUX
using Long = System.Int64;
#endif

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Vector
{
    private long _x;
    private long _y;

    public int X => (int) (_x >> 6);
    public int Y => (int) (_y >> 6);
}