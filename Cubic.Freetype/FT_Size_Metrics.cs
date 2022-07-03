using System.Runtime.InteropServices;
#if WINDOWS
using Long = System.Int32;
#elif LINUX
using Long = System.Int64;
#endif

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Size_Metrics
{
    public ushort XPPem;
    public ushort YPPem;

    public Long XScale;
    public Long YScale;

    public Long Ascender;
    public Long Descender;
    public Long Height;
    public Long MaxAdvance;
}