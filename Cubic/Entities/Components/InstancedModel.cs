using System;
using System.Collections.Generic;
using System.Numerics;
using Cubic.Graphics;
using Cubic.Primitives;
using Cubic.Render;
using Cubic.Render.Lighting;
using Cubic.Scenes;
using Cubic.Utilities;
using Silk.NET.OpenGL;
using Buffer = Cubic.Graphics.Buffer;
using Shader = Cubic.Render.Shader;

namespace Cubic.Entities.Components;

public class InstancedModel : Component
{
    private List<ModelGroup> _instances;
    private Shader _shader;

    public InstancedModel()
    {
        _instances = new List<ModelGroup>();
    }
    
    public unsafe ModelGroup CreateModelGroup(IPrimitive primitive, Material material)
    {
        GraphicsDevice device = CubicGraphics.GraphicsDevice;
        
        ModelGroup group = new ModelGroup()
        {
            VertexBuffer = device.CreateBuffer(BufferType.VertexBuffer, (uint) (primitive.Vertices.Length * sizeof(VertexPositionTextureNormal))),
            IndexBuffer = device.CreateBuffer(BufferType.IndexBuffer, (uint) (primitive.Indices.Length * sizeof(uint))),
            IndicesLength = primitive.Indices.Length,
            Material = material,
            ModelMatrices = new List<Matrix4x4>()
        };
        
        group.VertexBuffer.Update(0, primitive.Vertices);
        group.IndexBuffer.Update(0, primitive.Indices);

        _shader = new Shader(Model.VertexShader, Model.FragmentShader);

        _instances.Add(group);

        return group;
    }

    protected internal override unsafe void Draw()
    {
        base.Draw();
        
        // I AM AWARE THIS IS NOT INSTANCING!!!!
        // THIS IS MERELY TO GET A TEST WORKING

        foreach (ModelGroup modelGroup in _instances)
        {
            _shader.Set("uProjection", Camera.Main.ProjectionMatrix);
            _shader.Set("uView", Camera.Main.ViewMatrix);
            _shader.Set("uCameraPos", Camera.Main.Transform.Position);
            _shader.Set("uMaterial.albedo", 0);
            _shader.Set("uMaterial.specular", 1);
            _shader.Set("uMaterial.color", modelGroup.Material.Color);
            _shader.Set("uMaterial.shininess", modelGroup.Material.Shininess);
            DirectionalLight sun = SceneManager.Active.World.Sun;
            Vector3 sunColor = sun.Color.Normalize().ToVector3();
            float sunDegX = CubicMath.ToRadians(sun.Direction.X);
            float sunDegY = CubicMath.ToRadians(-sun.Direction.Y);
            _shader.Set("uSun.direction",
                new Vector3(MathF.Cos(sunDegX) * MathF.Cos(sunDegY), MathF.Cos(sunDegX) * MathF.Sin(sunDegY),
                    MathF.Sin(sunDegX)));
            _shader.Set("uSun.ambient", sunColor * sun.AmbientMultiplier);
            _shader.Set("uSun.diffuse", sunColor * sun.DiffuseMultiplier);
            _shader.Set("uSun.specular", sunColor * sun.SpecularMultiplier);

            GraphicsDevice device = CubicGraphics.GraphicsDevice;
            
            device.SetTexture(0, modelGroup.Material.Albedo.InternalTexture);
            device.SetTexture(1, modelGroup.Material.Specular.InternalTexture);
            
            device.SetShader(_shader.InternalProgram);
            
            device.SetVertexBuffer(modelGroup.VertexBuffer);
            device.SetIndexBuffer(modelGroup.IndexBuffer);

            foreach (Matrix4x4 mat in modelGroup.ModelMatrices)
            {
                _shader.Set("uModel", mat * Transform.TransformMatrix);
                device.Draw((uint) modelGroup.IndicesLength);
                Metrics.DrawCallsInternal++;
            }
        }
    }

    protected internal override void Unload()
    {
        base.Unload();

        foreach (ModelGroup group in _instances)
        {
            group.VertexBuffer.Dispose();
            group.IndexBuffer.Dispose();
        }
    }
}

public struct ModelGroup
{
    internal Buffer VertexBuffer;
    internal Buffer IndexBuffer;
    internal int IndicesLength;
    public Material Material;
    public List<Matrix4x4> ModelMatrices;

    public void AddMatrix(Matrix4x4 matrix)
    {
        ModelMatrices.Add(matrix);
    }

    public void RemoveMatrix(int index)
    {
        ModelMatrices.RemoveAt(index);
    }
}