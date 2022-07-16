using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Cubic.Entities;
using Cubic.Graphics;
using Cubic.Render.Lighting;
using Cubic.Scenes;
using Cubic.Utilities;
using Buffer = Cubic.Graphics.Buffer;

namespace Cubic.Render.Renderers;

public class ForwardRenderer : Renderer
{
    private List<Renderable> _opaques;
    private List<Renderable> _translucents;

    public ForwardRenderer()
    {
        _opaques = new List<Renderable>();
        _translucents = new List<Renderable>();
    }

    public override void RenderOpaque(Buffer vertexBuffer, Buffer indexBuffer, int numIndices, Matrix4x4 transform,
        Material material,
        Shader shader)
    {
        _opaques.Add(new Renderable(vertexBuffer, indexBuffer, numIndices, transform, material, shader));
    }

    public override void RenderTranslucent(Buffer vertexBuffer, Buffer indexBuffer, int numIndices, Matrix4x4 transform,
        Material material, Shader shader)
    {
        _translucents.Add(new Renderable(vertexBuffer, indexBuffer, numIndices, transform, material, shader));
    }

    internal override void PrepareForRender()
    {
        _opaques.Clear();
        _translucents.Clear();
    }

    internal override void PerformRenderPasses(Camera camera, Scene scene)
    {
        foreach (Renderable renderable in _opaques.OrderBy(pair =>
                     Vector3.Distance(pair.Transform.Translation, camera.Transform.Position)))
        {
            DrawRenderable(renderable, camera, scene);
        }
        
        foreach (Renderable renderable in _translucents.OrderBy(pair =>
                     -Vector3.Distance(pair.Transform.Translation, camera.Transform.Position)))
        {
            DrawRenderable(renderable, camera, scene);
        }
    }

    private void DrawRenderable(Renderable renderable, Camera camera, Scene scene)
    {
        renderable.Shader.Set("uProjection", camera.ProjectionMatrix);
        renderable.Shader.Set("uView", camera.ViewMatrix);
        renderable.Shader.Set("uModel", renderable.Transform);
        
        renderable.Shader.Set("uCameraPos", camera.Transform.Position);
        renderable.Shader.Set("uMaterial.albedo", 0);
        renderable.Shader.Set("uMaterial.specular", 1);
        renderable.Shader.Set("uMaterial.color", renderable.Material.Color);
        renderable.Shader.Set("uMaterial.shininess", renderable.Material.Shininess);
        DirectionalLight sun = scene.World.Sun;
        Vector3 sunColor = sun.Color.Normalize().ToVector3();
        float sunDegX = CubicMath.ToRadians(sun.Direction.X);
        float sunDegY = CubicMath.ToRadians(-sun.Direction.Y);
        renderable.Shader.Set("uSun.direction",
            new Vector3(MathF.Cos(sunDegX) * MathF.Cos(sunDegY), MathF.Cos(sunDegX) * MathF.Sin(sunDegY),
                MathF.Sin(sunDegX)));
        renderable.Shader.Set("uSun.ambient", sunColor * sun.AmbientMultiplier);
        renderable.Shader.Set("uSun.diffuse", sunColor * sun.DiffuseMultiplier);
        renderable.Shader.Set("uSun.specular", sunColor * sun.SpecularMultiplier);

        GraphicsDevice device = CubicGraphics.GraphicsDevice;
        
        device.SetTexture(0, renderable.Material.Albedo.InternalTexture);
        device.SetTexture(1, renderable.Material.Specular.InternalTexture);
        
        device.SetShader(renderable.Shader.InternalProgram);
        
        device.SetVertexBuffer(renderable.VertexBuffer);
        device.SetIndexBuffer(renderable.IndexBuffer);

        device.Draw((uint) renderable.NumIndices);
        Metrics.DrawCallsInternal++;
    }

    private struct Renderable
    {
        public Buffer VertexBuffer;
        public Buffer IndexBuffer;
        public int NumIndices;
        public Matrix4x4 Transform;
        public Material Material;
        public Shader Shader;

        public Renderable(Buffer vertexBuffer, Buffer indexBuffer, int numIndices, Matrix4x4 transform, Material material, Shader shader)
        {
            VertexBuffer = vertexBuffer;
            IndexBuffer = indexBuffer;
            NumIndices = numIndices;
            Transform = transform;
            Material = material;
            Shader = shader;
        }
    }
}