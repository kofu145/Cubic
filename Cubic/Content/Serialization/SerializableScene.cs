using System.Collections.Generic;
using Cubic.Entities;
using Cubic.Scenes;
using Cubic.Utilities;

namespace Cubic.Content.Serialization;

public struct SerializableScene
{
    public World World;

    public Dictionary<string, SerializableEntity> Entities;

    public SerializableScene()
    {
        World = new World();
        Entities = new Dictionary<string, SerializableEntity>();
    }

    public SerializableScene(Scene scene) : this()
    {
        foreach (Entity entity in scene.GetAllEntities())
            Entities.Add(entity.Name, new SerializableEntity(entity));
    }
}