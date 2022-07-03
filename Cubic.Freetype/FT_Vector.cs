using System.Runtime.InteropServices;

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Vector
{
    private int _x;
    private int _y;

    public int X => (int) (_x >> 6);
    public int Y => (int) (_y >> 6);
}