using System.Runtime.InteropServices;

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_BBox
{
    public int XMin;
    public int YMin;
    public int XMax;
    public int YMax;
}