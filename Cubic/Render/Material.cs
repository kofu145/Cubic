using System.Drawing;

namespace Cubic.Render;

public class Material
{
    private bool _translucent;
    private bool _valueSet;
    
    public Texture Albedo;
    public Texture Specular;
    public Color Color;
    public int Shininess;

    public bool Translucent
    {
        get => _valueSet ? _translucent : Color.A < 255;
        set
        {
            _translucent = value;
            _valueSet = true;
        }
    }

    public Material(Texture albedo, Texture specular, Color color, int shininess)
    {
        Albedo = albedo;
        Specular = specular;
        Color = color;
        Shininess = shininess;
    }

    public Material(Texture albedo) : this(albedo, albedo, Color.White, 1) { }
}