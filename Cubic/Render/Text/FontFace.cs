using System;
using Cubic.Freetype;
using static Cubic.Freetype.FreeType;

namespace Cubic.Render.Text;

internal unsafe struct FontFace : IDisposable
{
    public FT_Face* Face;
    
    public FontFace(string fontPath)
    {
        FT_Face* face;
        if (FT_New_Face(FontHelper.FreeType, fontPath, 0, (FT_Face*) &face) != 0)
            throw new CubicException("Font could not be loaded!");
        Face = face;
    }

    public FontFace(byte[] data)
    {
        FT_Face* face;
        fixed (byte* p = data)
        {
            if (FT_New_Memory_Face(FontHelper.FreeType, p, data.Length, 0, (FT_Face*) &face) != 0)
                throw new CubicException("Font could not be loaded!");
        }

        Face = face;
    }

    public void Dispose()
    {
        FT_Done_Face(Face);
    }
}