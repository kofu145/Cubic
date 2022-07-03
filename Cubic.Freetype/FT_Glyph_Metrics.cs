using System.Runtime.InteropServices;

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Glyph_Metrics
{
    public int Width;
    public int Height;

    public int HoriBearingX;
    public int HoriBearingY;
    public int HoriAdvance;

    public int VertBearingX;
    public int VertBearingY;
    public int VertAdvance;
}