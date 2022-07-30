using System;
using System.Numerics;
using Cubic.Entities;
using Cubic.Graphics;
using Cubic.Scenes;
using Buffer = Cubic.Graphics.Buffer;

namespace Cubic.Render.Renderers;

public abstract class Renderer : IDisposable
{
    public abstract void RenderOpaque(Buffer vertexBuffer, Buffer indexBuffer, int numIndices, Matrix4x4 transform,
        Material material, Shader shader, uint stride, ShaderLayout[] layout);
    
    public abstract void RenderTranslucent(Buffer vertexBuffer, Buffer indexBuffer, int numIndices, Matrix4x4 transform,
        Material material, Shader shader, uint stride, ShaderLayout[] layout);

    internal abstract void PrepareForRender();

    internal abstract void PerformRenderPasses(Camera camera, Scene scene);

    public abstract void Dispose();
}