using System.Runtime.InteropServices;
#if WINDOWS
using CLong = System.Int32;
#elif LINUX
using CLong = System.Int64;
#endif

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_BBox
{
    public CLong XMin;
    public CLong YMin;
    public CLong XMax;
    public CLong YMax;
}