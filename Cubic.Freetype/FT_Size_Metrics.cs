using System.Runtime.InteropServices;

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Size_Metrics
{
    public ushort XPPem;
    public ushort YPPem;

    public long XScale;
    public long YScale;

    public long Ascender;
    public long Descender;
    public long Height;
    public long MaxAdvance;
}