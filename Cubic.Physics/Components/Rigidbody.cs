using BulletSharp;
using Cubic.Entities.Components;

namespace Cubic.Physics.Components;

public class Rigidbody : Component
{
    private CollisionShape _shape;
    private float _mass;
    private RigidBody _body;
    
    public Rigidbody(CollisionShape shape, float mass)
    {
        _shape = shape;
        _mass = mass;
    }

    protected override void Initialize()
    {
        base.Initialize();

        _body = Physics.CreateBody(Transform, _mass, _shape);
    }

    protected override void Update()
    {
        base.Update();

        Transform.Position = _body.WorldTransform.Translation;
        Transform.Rotation = _body.Orientation;
    }
}