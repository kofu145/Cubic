using System.Collections.Generic;
using Cubic.Entities;
using Cubic.Entities.Components;

namespace Cubic.Content.Serialization;

public struct SerializableEntity
{
    public string Name;
    public Transform Transform;
    public List<Component> Components;

    public SerializableEntity(string name)
    {
        Name = name;
        Components = new List<Component>();
        Transform = new Transform();
    }

    public SerializableEntity(Entity entity) : this(entity.Name)
    {
        Transform = entity.Transform;
        foreach (Component component in entity.Components)
        {
            if (component != null)
                Components.Add(component);
        }
    }
}