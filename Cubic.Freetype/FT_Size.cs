using System.Runtime.InteropServices;

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FT_Size
{
    public FT_Face* Face;
    public FT_Generic Generic;
    public FT_Size_Metrics Metrics;
    public IntPtr Internal;
}