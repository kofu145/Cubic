using System.Numerics;
using BulletSharp;
using Cubic.Entities;
using Cubic.Render;
using Cubic.Windowing;

namespace Cubic.Physics;

public static class Physics
{
    public static readonly DiscreteDynamicsWorld World;
    private static readonly CollisionDispatcher _dispatcher;
    private static readonly DbvtBroadphase _broadphase;

    public static Vector3 Gravity
    {
        get => World.Gravity;
        set => World.Gravity = value;
    }

    static Physics()
    {
        using CollisionConfiguration conf = new DefaultCollisionConfiguration();
        _dispatcher = new CollisionDispatcher(conf);
        _broadphase = new DbvtBroadphase();

        World = new DiscreteDynamicsWorld(_dispatcher, _broadphase, null, conf);
        World.Gravity = new Vector3(0, -9.81f, 0);
    }

    public static void Initialize(CubicGame game)
    {
        game.GameUpdate += Update;
    }
    
    public static RigidBody CreateBody(Transform transform, float mass, CollisionShape shape)
    {
        Vector3 inertia = shape.CalculateLocalInertia(mass);
        using DefaultMotionState state = new DefaultMotionState(transform.TransformMatrix);
        using RigidBodyConstructionInfo inf = new RigidBodyConstructionInfo(mass, state, shape, inertia);
        RigidBody body = new RigidBody(inf);
        World.AddRigidBody(body);
        return body;
    }

    private static void Update(CubicGame game, CubicGraphics graphics)
    {
        World.StepSimulation(Time.DeltaTime);
    }
}