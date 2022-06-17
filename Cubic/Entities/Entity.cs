using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cubic.Entities.Components;
using Cubic.Windowing;

namespace Cubic.Entities;

public class Entity : IDisposable
{
    public string Name { get; internal set; }

    public string Tag;
    
    internal CubicGame Game;
    private bool _initialized;
    private bool _updating;
    
    public Transform Transform;
    
    internal Component[] Components;
    private List<ComponentState> _componentStates;
    private int _componentCount;

    public Entity() : this(new Transform()) { }

    public Entity(Transform transform)
    {
        Transform = transform;
        Components = new Component[5];
        _componentStates = new List<ComponentState>();
    }

    /// <summary>
    /// Add a component to this entity.
    /// </summary>
    /// <param name="component"></param>
    /// <param name="args"></param>
    /// <exception cref="Exception"></exception>
    /// <exception cref="CubicException"></exception>
    public void AddComponent(Component component)
    {
        foreach (Component comp in Components)
        {
            if (comp == null)
                continue;
            
            if (comp.GetType() == component.GetType())
                throw new CubicException("Entity can have only one type of each component.");
        }

        if (_updating)
            _componentStates.Add(new ComponentState(component, component.GetType(), true));
        else
            CreateComponent(component);
    }

    public void RemoveComponent(Type component)
    {
        if (component != typeof(Component) && component.BaseType != typeof(Component))
            throw new Exception($"Given component must be of type {typeof(Component)}");

        if (_updating)
        {
            _componentStates.Add(new ComponentState(null, component, false));
            return;
        }

        DeleteComponent(component);
    }

    public void RemoveComponent<T>() => RemoveComponent(typeof(T));

    public T GetComponent<T>() where T : Component
    {
        foreach (Component component in Components)
        {
            if (component?.GetType() == typeof(T))
                return (T) component;
        }

        return null;
    }

    public Component GetComponent(Type component)
    {
        if (component.BaseType != typeof(Component))
            throw new CubicException($"Given component must derive off {typeof(Component)}.");
        
        foreach (Component comp in Components)
        {
            if (comp?.GetType() == component)
                return comp;
        }

        return null;
    }

    protected internal virtual void Update()
    {
        _updating = true;
        foreach (Component component in Components)
        {
            if (component is not { Enabled: true })
                continue;
            component.Update();
        }

        _updating = false;

        foreach (ComponentState cState in _componentStates)
        {
            if (cState.Add)
                cState.Component.Initialize();
            else
                DeleteComponent(cState.ComponentType);
        }
        _componentStates.Clear();
    }

    protected internal virtual void Draw()
    {
        foreach (Component component in Components)
        {
            if (component is not { Enabled: true })
                continue;
            component.Draw();
        }
    }

    internal void Initialize(CubicGame game)
    {
        Game = game;
        
        foreach (Component comp in Components)
        {
            if (comp == null)
                continue;
            comp.Entity = this;
            comp.Initialize();
        }

        _initialized = true;
        
        Initialize();
    }

    protected virtual void Initialize() { }

    private void CreateComponent(Component comp)
    {
        if (_componentCount + 1 > Components.Length)
            Array.Resize(ref Components, Components.Length * 2);
        Components[_componentCount] = comp;
        _componentCount++;

        if (!_initialized)
            return;
        
        comp.Entity = this;
        comp.Initialize();
    }

    private void DeleteComponent(Type component)
    {
        for (int i = 0; i < Components.Length; i++)
        {
            if (Components[i]?.GetType() == component)
            {
                _componentCount--;
                // https://github.com/prime31/Nez/blob/master/Nez.Portable/Utils/Collections/FastList.cs#L103
                Array.Copy(Components, i + 1, Components, i, _componentCount - i);
                Components[_componentCount] = null;

                GC.Collect();

                return;
            }
        }

        throw new CubicException("Given component does not exist in the entity.");
    }

    private struct ComponentState
    {
        public Component Component;
        public Type ComponentType;
        public bool Add;

        public ComponentState(Component component, Type componentType, bool add)
        {
            Component = component;
            Add = add;
            ComponentType = componentType;
        }
    }

    public void Dispose()
    {
        foreach (Component comp in Components)
            comp?.Unload();
    }
}