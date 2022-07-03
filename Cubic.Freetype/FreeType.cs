using System.Runtime.InteropServices;

#if WINDOWS
using CLong = System.Int32;
#elif LINUX
using CLong = System.Int64;
#endif

namespace Cubic.Freetype;

public static unsafe class FreeType
{
    private const string LibraryName = "libfreetype";

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int FT_Init_FreeType(IntPtr* library);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int FT_Done_FreeType(IntPtr library);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int FT_New_Face(IntPtr library, string path, CLong index, FT_Face* face);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int FT_New_Memory_Face(IntPtr library, byte* file, CLong size, CLong index, FT_Face* face);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int FT_Done_Face(FT_Face* face);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int FT_Set_Pixel_Sizes(FT_Face* face, uint width, uint height);
    
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int FT_Load_Char(FT_Face* face, uint c, FTLoad flags);
}