using System;
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

public class Model : Component
{
    public readonly VertexPositionTextureNormal[] Vertices;
    public readonly uint[] Indices;

    private uint _vao;
    private Buffer _vertexBuffer;
    private Buffer _indexBuffer;

    private static Shader _shader;
    private static bool _shaderDisposed;

    public Material Material;

    public const string VertexShader = @"
in vec3 aPosition;
in vec2 aTexCoords;
in vec3 aNormals;

out vec2 frag_texCoords;
out vec3 frag_normal;
out vec3 frag_position;

uniform mat4 uModel;
uniform mat4 uCamera;

void main()
{
    frag_texCoords = aTexCoords;
    frag_position = vec3(vec4(aPosition, 1.0) * uModel);
    gl_Position = vec4(frag_position, 1.0) * uCamera;
    frag_normal = aNormals * mat3(transpose(inverse(uModel)));
}";

    public const string FragmentShader = @"
struct Material
{
    sampler2D albedo;
    sampler2D specular;
    vec4 color;
    int shininess;
};

struct DirectionalLight
{
    vec3 direction;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

in vec2 frag_texCoords;
in vec3 frag_normal;
in vec3 frag_position;

out vec4 out_color;

uniform DirectionalLight uSun;
uniform Material uMaterial;
uniform vec3 uCameraPos;
//uniform sampler2D uShadowMap;
//uniform vec4 uLightSpace;

vec4 CalculateDirectional(DirectionalLight light, vec3 normal, vec3 viewDir, float shadow);
float CalculateShadow(vec4 lightSpace);

void main()
{
    vec3 norm = normalize(frag_normal);
    vec3 viewDir = normalize(uCameraPos - frag_position);
    
    vec4 result = CalculateDirectional(uSun, norm, viewDir, 0.0 /*CalculateShadow(vec4(frag_position, 1.0) * uLightSpace)*/);
    out_color = result * uMaterial.color;
}

vec4 CalculateDirectional(DirectionalLight light, vec3 normal, vec3 viewDir, float shadow)
{
    vec3 lightDir = normalize(-light.direction);
    
    float diff = max(dot(normal, lightDir), 0.0);
    
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), uMaterial.shininess);

    vec4 alRes = texture(uMaterial.albedo, frag_texCoords);

    vec3 ambient = light.ambient * vec3(alRes);
    vec3 diffuse = light.diffuse * diff * vec3(alRes);
    vec3 specular = light.specular * spec * vec3(texture(uMaterial.specular, frag_texCoords));
    //return vec4(ambient + diffuse + specular, alRes.a);
    return vec4(ambient + (1.0 - shadow) * (diffuse + specular), alRes.a);
}

/*float CalculateShadow(vec4 lightSpace)
{   
    //vec3 proj = lightSpace.xyz / lightSpace.w;
    vec3 proj = vec3(lightSpace);
    if (proj.z > 1.0)
        return 0.0;
    vec2 coord = proj.xy * 0.5 + 0.5;
    float closestDepth = texture(uShadowMap, coord).r;
    float currentDepth = proj.z;
    //float bias = max(0.05 * (1.0 - dot(frag_normal, uSun.direction)), 0.005);
    float shadow = currentDepth > closestDepth ? 1.0 : 0.0;

    return shadow;
}*/";

    public static readonly ShaderLayout[] Layout;
    public const uint Stride = 32;

    static Model()
    {
        _shaderDisposed = true;
        Layout = new[]
        {
            new ShaderLayout("aPosition", 3, AttribType.Float),
            new ShaderLayout("aTexCoords", 2, AttribType.Float),
            new ShaderLayout("aNormals", 3, AttribType.Float)
        };
    }

    public Model(VertexPositionTextureNormal[] vertices, uint[] indices)
    {
        Vertices = vertices;
        Indices = indices;
    }

    public Model(IPrimitive primitive, Material material)
    {
        Vertices = primitive.Vertices;
        Indices = primitive.Indices;
        Material = material;
    }

    protected internal override unsafe void Initialize()
    {
        base.Initialize();

        if (_shaderDisposed)
        {
            _shader = new Shader(VertexShader, FragmentShader);
            _shaderDisposed = false;
        }

        GraphicsDevice device = CubicGraphics.GraphicsDevice;

        _vertexBuffer = device.CreateBuffer(BufferType.VertexBuffer,
            (uint) (Vertices.Length * sizeof(VertexPositionTextureNormal)));
        _vertexBuffer.Update(0, Vertices);

        _indexBuffer = device.CreateBuffer(BufferType.IndexBuffer, (uint) (Indices.Length * sizeof(uint)));
        _indexBuffer.Update(0, Indices);
    }

    protected internal override unsafe void Draw()
    {
        base.Draw();
        
        _shader.Set("uMaterial.albedo", 0);
        _shader.Set("uMaterial.specular", 1);
        //_shader.Set("uShadowMap", 2);

        if (Material.Translucent)
        {
            SceneManager.Active.Renderer.RenderTranslucent(_vertexBuffer, _indexBuffer, Indices.Length,
                Transform.TransformMatrix, Material, _shader, Stride, Layout);
        }
        else
        {
            SceneManager.Active.Renderer.RenderOpaque(_vertexBuffer, _indexBuffer, Indices.Length,
                Transform.TransformMatrix, Material, _shader, Stride, Layout);
        }
    }

    protected internal override void Unload()
    {
        base.Unload();

        if (!_shaderDisposed)
        {
            _shaderDisposed = true;
            _shader.Dispose();
        }
    }
}