using System.Numerics;
using Cubic.Utilities;

namespace Cubic.Entities;

public class Camera2D : Entity
{
    public bool UseCustomTransformMatrix;

    public Matrix4x4 TransformMatrix;

    public Camera2D()
    {
        UseCustomTransformMatrix = false;
    }

    internal void GenerateTransformMatrix()
    {
        if (UseCustomTransformMatrix)
            return;
        TransformMatrix = Matrix4x4.CreateTranslation(-Transform.Position) * 
                          Matrix4x4.CreateTranslation(new Vector3(-Transform.Origin, 0)) * 
                          Matrix4x4.CreateFromQuaternion(Transform.Rotation) * 
                          Matrix4x4.CreateScale(Transform.Scale) * 
                          Matrix4x4.CreateTranslation(new Vector3(Transform.Origin, 0));
    }
    
    public static Camera2D Main { get; internal set; }
}