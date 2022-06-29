using System.Runtime.InteropServices;

namespace Cubic.Freetype;

public static unsafe class FreeType
{
    private const string LibraryName = "libfreetype";

    [DllImport(LibraryName)]
    public static extern int FT_Init_FreeType(out IntPtr library);

    [DllImport(LibraryName)]
    public static extern int FT_New_Face(IntPtr library, string path, long index, out FT_Face face);
}