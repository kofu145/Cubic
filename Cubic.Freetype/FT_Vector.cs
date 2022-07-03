using System.Runtime.InteropServices;

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Vector
{
    private CLong _x;
    private CLong _y;

    public int X => (int) (_x.Value >> 6);
    public int Y => (int) (_y.Value >> 6);
}