using System.Collections.Generic;
using Cubic.Entities;
using Cubic.Entities.Components;

namespace Cubic.Content.Serialization;

public struct SerializableEntity
{
    public List<Component> Components;

    public SerializableEntity()
    {
        Components = new List<Component>();
    }

    public SerializableEntity(Entity entity) : this()
    {
        foreach (Component component in entity.Components)
        {
            if (component != null)
                Components.Add(component);
        }
    }
}