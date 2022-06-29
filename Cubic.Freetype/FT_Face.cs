using System.Runtime.InteropServices;

namespace Cubic.Freetype;

[StructLayout(LayoutKind.Sequential)]
public struct FT_Face
{
    public long NumFaces;
    public long FaceIndex;

    public long FaceFlags;
    public long StyleFlags;

    public long NumGlyphs;

    public string FamilyName;
    public string StyleName;

    public int NumFixedSizes;
    public FT_Bitmap_Size[] AvailableSizes;

    public int NumCharmaps;
    public FT_Charmap[] Charmaps;
    
    
}