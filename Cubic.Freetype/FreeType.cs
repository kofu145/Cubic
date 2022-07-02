using System.Runtime.InteropServices;

namespace Cubic.Freetype;

public static unsafe class FreeType
{
    private const string LibraryName = "libfreetype";

    public const int LoadDefault = 0;
    public const int LoadRender = 1 << 2;

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int FT_Init_FreeType(IntPtr* library);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int FT_Done_FreeType(IntPtr library);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int FT_New_Face(IntPtr library, string path, long index, FT_Face* face);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int FT_Done_Face(FT_Face* face);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int FT_Set_Pixel_Sizes(FT_Face* face, uint width, uint height);
    
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int FT_Load_Char(FT_Face* face, uint c, int flags);
}