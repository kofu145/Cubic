using System.Runtime.InteropServices;

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Glyph_Metrics
{
    public long Width;
    public long Height;

    public long HoriBearingX;
    public long HoriBearingY;
    public long HoriAdvance;

    public long VertBearingX;
    public long VertBearingY;
    public long VertAdvance;
}