using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Cubic.Entities;
using Cubic.Graphics;
using Cubic.Render.Renderers;
using Cubic.Scenes;

namespace Cubic.Render.PostProcessing;

public class ShadowMap : IDisposable
{
    private Framebuffer _framebuffer;
    public Cubic.Graphics.Texture DepthTexture;

    public const string ShadowVertex = @"
in vec3 aPosition;
in vec2 aTexCoords;
in vec3 aNormal;

uniform mat4 uModel;
uniform mat4 uLightSpace;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * uModel * uLightSpace;
}";

    public const string ShadowFragment = Shader.Empty;

    private Shader _shader;
    private ShaderLayout[] _layouts;
    
    public ShadowMap(Size size)
    {
        GraphicsDevice device = CubicGraphics.GraphicsDevice;

        _framebuffer = device.CreateFramebuffer();
        DepthTexture = device.CreateTexture((uint) size.Width, (uint) size.Height, PixelFormat.DepthOnly,
            TextureSample.Nearest, false, TextureUsage.Framebuffer, TextureWrap.ClampToBorder);
        DepthTexture.BorderColor = Color.White;
        _framebuffer.AttachTexture(DepthTexture);

        _shader = new Shader(ShadowVertex, ShadowFragment);
        _layouts = new[]
        {
            new ShaderLayout("aPosition", 3, AttribType.Float),
            new ShaderLayout("aTexCoords", 2, AttribType.Float),
            new ShaderLayout("aNormal", 3, AttribType.Float)
        };
    }

    internal Matrix4x4 Draw(Scene scene, Camera camera, List<ForwardRenderer.Renderable> renderables)
    {
        Matrix4x4 projection = Matrix4x4.CreateOrthographicOffCenter(-10.0f, 10.0f, -10.0f, 10, 1.0f, 200f);
        Matrix4x4 view = Matrix4x4.CreateLookAt(-scene.World.Sun.Forward, Vector3.Zero, Vector3.UnitY);
        Matrix4x4 space = view * projection;

        GraphicsDevice device = CubicGraphics.GraphicsDevice;
        //device.Options.CullFace = CullFace.Front;
        device.SetFramebuffer(_framebuffer);
        device.Clear(Vector4.Zero, ClearFlags.Depth);
        device.SetShader(_shader.InternalProgram);
        _shader.Set("uLightSpace", space);
        
        foreach (ForwardRenderer.Renderable renderable in renderables.OrderBy(pair =>
                     Vector3.Distance(pair.Transform.Translation, camera.Transform.Position)))
        {
            DrawRenderable(renderable);
        }
        
        device.SetFramebuffer(null);

        //device.Options.CullFace = CullFace.Back;
        
        //if (renderables.Count > 0)
        //    renderables.Add(new ForwardRenderer.Renderable(renderables[^1].VertexBuffer, renderables[^1].IndexBuffer, renderables[^1].NumIndices, Matrix4x4.CreateTranslation(-scene.World.Sun.Forward * 10), renderables[^1].Material, renderables[^1].Shader, renderables[^1].Stride, renderables[^1].Layout));

        return space;
    }

    private void DrawRenderable(ForwardRenderer.Renderable renderable)
    {
        GraphicsDevice device = CubicGraphics.GraphicsDevice;

        _shader.Set("uModel", renderable.Transform);

        device.SetVertexBuffer(renderable.VertexBuffer, 32, _layouts);
        device.SetIndexBuffer(renderable.IndexBuffer);

        device.Draw((uint) renderable.NumIndices);
        Metrics.DrawCallsInternal++;
    }

    public void Dispose()
    {
        _framebuffer.Dispose();
        DepthTexture.Dispose();
    }
}