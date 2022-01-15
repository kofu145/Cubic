using Cubic2D.Render;

namespace Cubic2D.Entities.Components;

public abstract class Component
{
    protected internal Entity Entity { get; internal set; }

    protected Transform Transform => Entity.Transform;
    
    protected internal virtual void Initialize() { }

    protected internal virtual void Update() { }

    protected internal virtual void Draw(Graphics graphics) { }

    protected T GetComponent<T>() where T : Component => (T) Entity.GetComponent<T>();

    protected void AddComponent(Component component) => Entity.AddComponent(component);
}