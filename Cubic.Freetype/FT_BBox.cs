using System.Runtime.InteropServices;
#if WINDOWS
using Long = System.Int32;
#elif LINUX
using Long = System.Int64;
#endif

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_BBox
{
    public Long XMin;
    public Long YMin;
    public Long XMax;
    public Long YMax;
}