using System.Runtime.InteropServices;

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Size_Metrics
{
    public ushort XPPem;
    public ushort YPPem;

    public int XScale;
    public int YScale;

    public int Ascender;
    public int Descender;
    public int Height;
    public int MaxAdvance;
}