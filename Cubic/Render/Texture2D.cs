using System;
using System.Drawing;
using System.IO;
using System.Text.Json.Serialization;
using Cubic.Graphics;
using Cubic.Utilities;
using Cubic.Windowing;
using Silk.NET.OpenGL;
using StbImageSharp;
using static Cubic.Render.CubicGraphics;
using PixelFormat = Cubic.Graphics.PixelFormat;

namespace Cubic.Render;

public class Texture2D : Texture
{
    /// <summary>
    /// The path, if any, of this texture. Useful for JSON serialization.
    /// </summary>
    public string Path { get; }

    [JsonConstructor]
    public Texture2D(string path, bool autoDispose = true) : base(autoDispose)
    {
        ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(path));
            
        InternalTexture = CreateTexture(result.Width, result.Height, result.Data,
            result.Comp == ColorComponents.RedGreenBlueAlpha ? PixelFormat.RGBA : PixelFormat.RGB);
        Size = new Size(result.Width, result.Height);
        Path = path;
    }

    public Texture2D(int width, int height, byte[] data, bool autoDispose = true) : base(autoDispose)
    {
        InternalTexture = CreateTexture(width, height, data);
        Size = new Size(width, height);
    }

    public Texture2D(int width, int height, bool autoDispose = true) : base(autoDispose)
    {
        InternalTexture = CreateTexture(width, height, null);
        Size = new Size(width, height);
    }

    public Texture2D(Bitmap bitmap, bool autoDispose = true) : base(autoDispose)
    {
        InternalTexture = CreateTexture(bitmap.Size.Width, bitmap.Size.Height, bitmap.Data, (PixelFormat) bitmap.ColorSpace - 1);
        Size = bitmap.Size;
    }

    public unsafe void SetData(IntPtr data, int x, int y, int width, int height)
    {
        InternalTexture.Update(x, y, (uint) width, (uint) height, data);
    }

    public unsafe void SetData(byte[] data, int x, int y, int width, int height)
    {
        InternalTexture.Update(x, y, (uint) width, (uint) height, data);
    }

    private static unsafe Cubic.Graphics.Texture CreateTexture(int width, int height, byte[] data, PixelFormat format = PixelFormat.RGBA)
    {
        GraphicsDevice device = CubicGraphics.GraphicsDevice;
        Cubic.Graphics.Texture tex = device.CreateTexture((uint) width, (uint) height, format, mipmap: false, anisotropicLevel: 16);
        if (data != null)
        {
            tex.Update(0, 0, (uint) width, (uint) height, data);
            tex.GenerateMipmaps();
        }

        return tex;
    }

    public static Texture2D Blank { get; internal set; }

    public static Texture2D Void { get; internal set; }
}