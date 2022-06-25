using System;
using System.Numerics;
using Cubic.Utilities;
using Newtonsoft.Json;

namespace Cubic.Entities;

[JsonObject(MemberSerialization.Fields)]
public class Transform
{
    /// <summary>
    /// The position of this transform.
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// The rotation of this transform.
    /// </summary>
    public Quaternion Rotation;
    
    /// <summary>
    /// The scale of this transform.
    /// </summary>
    public Vector3 Scale;

    /// <summary>
    /// The origin point of this transform.
    /// </summary>
    /// <remarks>This ONLY works on 2D entities.</remarks>
    public Vector2 Origin;

    public Vector3 Forward => Vector3.Transform(Vector3.UnitZ, Rotation);

    public Vector3 Backward => Vector3.Transform(-Vector3.UnitZ, Rotation);

    public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation);

    public Vector3 Left => Vector3.Transform(-Vector3.UnitX, Rotation);

    public Vector3 Up => Vector3.Transform(Vector3.UnitY, Rotation);

    public Vector3 Down => Vector3.Transform(-Vector3.UnitY, Rotation);

    public float SpriteRotation
    {
        get => Rotation.ToEulerAngles().Z;
        set => Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, value);
    }

    public Matrix4x4 TransformMatrix => Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateFromQuaternion(Rotation) *
                                        Matrix4x4.CreateTranslation(Position);

    public Transform()
    {
        Position = Vector3.Zero;
        Rotation = Quaternion.Identity;
        Scale = Vector3.One;
        Origin = Vector2.Zero;
    }

    public float LookAt(Vector2 position)
    {
        Vector2 diff = Position.ToVector2() - position;
        return MathF.Atan2(-diff.Y, -diff.X);
    }
}