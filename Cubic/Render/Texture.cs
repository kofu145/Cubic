using System;
using System.Drawing;
using System.Text.Json.Serialization;
using Cubic.Scenes;
using Silk.NET.OpenGL;
using static Cubic.Render.CubicGraphics;

namespace Cubic.Render;

public abstract class Texture : IDisposable
{
    internal Cubic.Graphics.Texture Tex;
    
    [JsonIgnore]
    public Size Size { get; protected set; }

    public Texture(bool autoDispose)
    {
        if (autoDispose)
             SceneManager.Active.CreatedResources.Add(this);
    }

    public virtual void Dispose()
    {
        Tex.Dispose();
#if DEBUG
        Console.WriteLine("Texture disposed");
#endif
    }
}