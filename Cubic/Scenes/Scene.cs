using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Cubic.Entities;
using Cubic.Entities.Components;
using Cubic.GUI;
using Cubic.Render;
using Cubic.Render.Renderers;
using Cubic.Utilities;
using Cubic.Windowing;

namespace Cubic.Scenes;

public abstract class Scene : IDisposable
{
    public string Name { get; internal set; }
    
    internal readonly List<IDisposable> CreatedResources;

    private bool _updating;

    public readonly Renderer Renderer;

    protected internal CubicGame Game { get; internal set; }
    protected CubicGraphics Graphics => CubicGame.GraphicsInternal;
    protected internal World World;

    private readonly Dictionary<string, Entity> _entitiesQueue;
    private readonly List<string> _entitiesToRemove;
    private readonly Dictionary<string, Entity> _entities;
    private readonly Dictionary<string, Screen> _screens;
    private Queue<Screen> _screensToAdd;
    private List<Screen> _activeScreens;
    private int _popCount;

    private int _uniqueId;

    public int EntityCount => _entities.Count;

    protected Scene()
    {
        CreatedResources = new List<IDisposable>();
        _entities = new Dictionary<string, Entity>();
        _entitiesQueue = new Dictionary<string, Entity>();
        _entitiesToRemove = new List<string>();
        _screens = new Dictionary<string, Screen>();
        World = new World();
        _activeScreens = new List<Screen>();
        _screensToAdd = new Queue<Screen>();
        _uniqueId = 0;
        Renderer = new ForwardRenderer();
    }

    protected internal virtual void Initialize() { }

    protected internal virtual void Update()
    {
        _updating = true;
        foreach ((_, Entity entity) in _entities)
        {
            if (entity.Enabled)
                entity.Update();
        }

        foreach (Screen screen in _activeScreens)
            screen.Update();
        _updating = false;

        foreach (KeyValuePair<string, Entity> ent in _entitiesQueue)
        {
            _entities.Add(ent.Key, ent.Value);
        }
        _entitiesQueue.Clear();

        foreach (string name in _entitiesToRemove)
            _entities.Remove(name);
        _entitiesToRemove.Clear();

        while (_screensToAdd.TryDequeue(out Screen result))
            _activeScreens.Add(result);

        for (int i = 0; i < _popCount; i++)
            _activeScreens.RemoveAt(_activeScreens.Count - 1);
        _popCount = 0;
    }

    protected virtual void Unload() { }

    public void Dispose()
    {
        Unload();
        // Stop all sounds from playing.
        for (int i = 0; i < Game.AudioDevice.NumChannels; i++)
            Game.AudioDevice.Stop(i);
        
        foreach (Entity entity in _entities.Values)
            entity.Dispose();
        
        foreach (IDisposable resource in CreatedResources)
            resource.Dispose();
        
        //World.Skybox?.Dispose();
    }

    /// <summary>
    /// Extend Cubic's graphics systems using this method.
    ///
    /// In order to get the engine to draw entities in the scene like normal, you <b>MUST</b> call base.Draw() somewhere
    /// within this method.
    /// </summary>
    protected internal virtual void Draw()
    {
        Camera.Main.GenerateViewMatrix();
        Camera2D.Main.GenerateTransformMatrix();
        World.Skybox?.Draw(Camera.Main);
        Graphics.SpriteRenderer.Begin(Camera2D.Main.TransformMatrix, World.SampleType);
        
        // Order the entities by their distance to the camera to support transparent sorting.
        //foreach (KeyValuePair<string, Entity> entity in _entities.OrderBy(pair => -Vector3.Distance(pair.Value.Transform.Position, Camera.Main.Transform.Position)))
        //    entity.Value.Draw();
        
        Renderer.PrepareForRender();
        foreach ((_, Entity entity) in _entities)
        {
            if (entity.Enabled)
                entity.Draw();
        }

        Renderer.PerformRenderPasses(Camera.Main, this);
        Graphics.SpriteRenderer.End();
        
        foreach (Screen screen in _activeScreens)
            screen.Draw();
    }

    public void AddEntity(string name, Entity entity)
    {
        entity.Name = name;
        entity.Initialize(Game);
        
        if (_updating)
        {
            _entitiesQueue.Add(name, entity);
            return;
        }
        _entities.Add(name, entity);
    }

    public void AddEntity(Entity entity) => AddEntity(_uniqueId++.ToString(), entity);
    
    public void RemoveEntity(string name)
    {
        if (_updating)
        {
            _entitiesToRemove.Add(name);
            return;
        }

        _entities.Remove(name);
    }

    public Entity GetEntity(string name) => _entities[name];

    public T GetEntity<T>(string name) where T : Entity => (T) _entities[name];

    public Entity[] GetEntitiesWithComponent<T>() where T : Component
    {
        List<Entity> entities = new List<Entity>();
        foreach (KeyValuePair<string, Entity> entity in _entities)
        {
            if (entity.Value.GetComponent<T>() != null)
                entities.Add(entity.Value);
        }

        return entities.ToArray();
    }

    public Entity[] GetEntitiesWithTag(string tag)
    {
        List<Entity> entities = new List<Entity>();
        foreach (KeyValuePair<string, Entity> entity in _entities)
        {
            if (entity.Value.Tag == tag)
                entities.Add(entity.Value);
        }

        return entities.ToArray();
    }

    public Entity[] GetAllEntities() => _entities.Values.ToArray();

    public void AddScreen(Screen screen, string name)
    {
        screen.Game = Game;
        screen.Initialize();
        _screens.Add(name, screen);
    }

    public void OpenScreen(string name)
    {
        Screen screen = _screens[name];
        screen.Open();
        if (_updating)
            _screensToAdd.Enqueue(screen);
        else
            _activeScreens.Add(screen);
    }

    public void CloseScreen()
    {
        _activeScreens[^1].Close();
        _popCount++;
    }

    internal void CloseScreenInternal()
    {
        _popCount++;
    }
}