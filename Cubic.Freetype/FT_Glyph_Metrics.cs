using System.Runtime.InteropServices;

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Glyph_Metrics
{
    public CLong Width;
    public CLong Height;

    public CLong HoriBearingX;
    public CLong HoriBearingY;
    public CLong HoriAdvance;

    public CLong VertBearingX;
    public CLong VertBearingY;
    public CLong VertAdvance;
}