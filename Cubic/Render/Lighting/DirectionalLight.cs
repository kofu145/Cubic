using System;
using System.Drawing;
using System.Numerics;
using Cubic.Utilities;

namespace Cubic.Render.Lighting;

public struct DirectionalLight
{
    public Vector2 Direction;
    public Color Color;
    public float AmbientMultiplier;
    public float DiffuseMultiplier;
    public float SpecularMultiplier;
    
    /// <summary>
    /// Calculate the forward vector of the light.
    /// </summary>
    public Vector3 Forward
    {
        get
        {
            float sunDegX = CubicMath.ToRadians(Direction.X);
            float sunDegY = CubicMath.ToRadians(-Direction.Y);

            return new Vector3(MathF.Cos(sunDegX) * MathF.Cos(sunDegY), MathF.Cos(sunDegX) * MathF.Sin(sunDegY),
                MathF.Sin(sunDegX));
        }
    }

    public DirectionalLight(Vector2 direction, Color color, float ambientMultiplier, float diffuseMultiplier, float specularMultiplier)
    {
        Direction = direction;
        Color = color;
        AmbientMultiplier = ambientMultiplier;
        DiffuseMultiplier = diffuseMultiplier;
        SpecularMultiplier = specularMultiplier;
    }
}