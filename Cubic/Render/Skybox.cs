using System;
using System.Numerics;
using Cubic.Entities;
using Cubic.Graphics;
using Cubic.Utilities;
using Silk.NET.OpenGL;
using Buffer = Cubic.Graphics.Buffer;

namespace Cubic.Render;

public class Skybox : IDisposable
{
    private readonly VertexPosition[] _vertices = new[]
    {
        new VertexPosition(new Vector3(-1, 1, -1)),
        new VertexPosition(new Vector3(1, 1, -1)),
        new VertexPosition(new Vector3(1, 1, 1)),
        new VertexPosition(new Vector3(-1, 1, 1)),

        new VertexPosition(new Vector3(-1, -1, 1)),
        new VertexPosition(new Vector3(1, -1, 1)),
        new VertexPosition(new Vector3(1, -1, -1)),
        new VertexPosition(new Vector3(-1, -1, -1)),

        new VertexPosition(new Vector3(-1, 1, -1)),
        new VertexPosition(new Vector3(-1, 1, 1)),
        new VertexPosition(new Vector3(-1, -1, 1)),
        new VertexPosition(new Vector3(-1, -1, -1)),

        new VertexPosition(new Vector3(1, 1, 1)),
        new VertexPosition(new Vector3(1, 1, -1)),
        new VertexPosition(new Vector3(1, -1, -1)),
        new VertexPosition(new Vector3(1, -1, 1)),

        new VertexPosition(new Vector3(1, 1, -1)),
        new VertexPosition(new Vector3(-1, 1, -1)),
        new VertexPosition(new Vector3(-1, -1, -1)),
        new VertexPosition(new Vector3(1, -1, -1)),

        new VertexPosition(new Vector3(-1, 1, 1)),
        new VertexPosition(new Vector3(1, 1, 1)),
        new VertexPosition(new Vector3(1, -1, 1)),
        new VertexPosition(new Vector3(-1, -1, 1))
    };
    
    private readonly uint[] _indices =
    {
        0, 1, 2, 0, 2, 3,
        4, 5, 6, 4, 6, 7,
        8, 9, 10, 8, 10, 11,
        12, 13, 14, 12, 14, 15,
        16, 17, 18, 16, 18, 19,
        20, 21, 22, 20, 22, 23
    };

    public const string VertexShader = @"
layout (location = 0) in vec3 aPosition;

out vec3 frag_texCoords;

uniform mat4 uView;
uniform mat4 uProjection;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * uView * uProjection;
    frag_texCoords = aPosition;
}";

    public const string FragmentShader = @"
in vec3 frag_texCoords;

out vec4 out_color;

uniform samplerCube uSkybox;

void main()
{
    out_color = texture(uSkybox, frag_texCoords);
}";

    private Buffer _vertexBuffer;
    private Buffer _indexBuffer;
    private Shader _shader;
    private CubeMap _cubeMap;

    public unsafe Skybox(CubeMap cubeMap)
    {
        _cubeMap = cubeMap;

        GraphicsDevice device = CubicGraphics.GraphicsDevice;

        _vertexBuffer =
            device.CreateBuffer(BufferType.VertexBuffer, (uint) (_vertices.Length * sizeof(VertexPosition)));
        _vertexBuffer.Update(0, _vertices);

        _indexBuffer = device.CreateBuffer(BufferType.IndexBuffer, (uint) (_indices.Length * sizeof(uint)));
        _indexBuffer.Update(0, _indices);

        _shader = new Shader(VertexShader, FragmentShader);
    }

    internal unsafe void Draw(Camera camera)
    {
        GraphicsDevice device = CubicGraphics.GraphicsDevice;
        device.Options.CullFace = CullFace.Front;
        device.Options.DepthMask = false;
        device.SetShader(_shader.Program);
        Matrix4x4 view = camera.ViewMatrix;
        _shader.Set("uProjection", camera.ProjectionMatrix);
        // Convert the camera's 4x4 view matrix to a 3x3 rotation matrix - we only need rotation, not translation.
        _shader.Set("uView", camera.ViewMatrix.To3x3Matrix());
        
        device.SetTexture(0, _cubeMap.Tex);
        device.DrawElements((uint) _indices.Length);

        device.Options.CullFace = CullFace.Back;
        device.Options.DepthMask = true;
    }

    public void Dispose()
    {
        _cubeMap.Dispose();
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _shader.Dispose();
    }
}