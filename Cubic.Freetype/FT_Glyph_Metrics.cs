using System.Runtime.InteropServices;
#if WINDOWS
using Long = System.Int32;
#elif LINUX
using Long = System.Int64;
#endif

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Glyph_Metrics
{
    public Long Width;
    public Long Height;

    public Long HoriBearingX;
    public Long HoriBearingY;
    public Long HoriAdvance;

    public Long VertBearingX;
    public Long VertBearingY;
    public Long VertAdvance;
}