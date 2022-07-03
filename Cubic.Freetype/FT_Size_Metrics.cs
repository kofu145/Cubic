using System.Runtime.InteropServices;

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Size_Metrics
{
    public ushort XPPem;
    public ushort YPPem;

    public CLong XScale;
    public CLong YScale;

    public CLong Ascender;
    public CLong Descender;
    public CLong Height;
    public CLong MaxAdvance;
}