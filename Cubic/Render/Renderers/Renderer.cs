using System.Numerics;
using Cubic.Entities;
using Cubic.Graphics;
using Cubic.Scenes;

namespace Cubic.Render.Renderers;

public abstract class Renderer
{
    public abstract void RenderOpaque(Buffer vertexBuffer, Buffer indexBuffer, int numIndices, Matrix4x4 transform,
        Material material,
        Shader shader);
    
    public abstract void RenderTranslucent(Buffer vertexBuffer, Buffer indexBuffer, int numIndices, Matrix4x4 transform,
        Material material, Shader shader);
    
    internal abstract void PrepareForRender();
    
    internal abstract void PerformRenderPasses(Camera camera, Scene scene);
}