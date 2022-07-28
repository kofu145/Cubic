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
        // todo: This has not been fixed for several months. Please actually be bothered to fix it SOON PLEASE THANKS

        foreach (ModelGroup modelGroup in _instances)
        {
            foreach (Matrix4x4 mat in modelGroup.ModelMatrices)
            {
                if (modelGroup.Material.Translucent)
                {
                    SceneManager.Active.Renderer.RenderTranslucent(modelGroup.VertexBuffer, modelGroup.IndexBuffer,
                        modelGroup.IndicesLength, mat * Transform.TransformMatrix, modelGroup.Material, _shader,
                        Model.Stride, Model.Layout);
                }
                else
                {
                    SceneManager.Active.Renderer.RenderOpaque(modelGroup.VertexBuffer, modelGroup.IndexBuffer,
                        modelGroup.IndicesLength, mat * Transform.TransformMatrix, modelGroup.Material, _shader,
                        Model.Stride, Model.Layout);
                }
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

    public void ClearAllMatrices()
    {
        ModelMatrices.Clear();
    }
}