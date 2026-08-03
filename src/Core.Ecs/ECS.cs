namespace Core.Ecs;

public readonly record struct EntityId(uint Value)
{
    public static readonly EntityId Null = new(uint.MaxValue);
}

public interface IComponent { }

public class World
{
    private uint _nextEntityId = 0;
    private readonly Queue<uint> _recycledIds = new();
    private readonly Dictionary<Type, IComponentPool> _pools = new();

    public EntityId CreateEntity()
    {
        if (_recycledIds.Count > 0)
        {
            return new EntityId(_recycledIds.Dequeue());
        }
        return new EntityId(_nextEntityId++);
    }

    public void DestroyEntity(EntityId entity)
    {
        foreach (var pool in _pools.Values)
        {
            pool.Remove(entity);
        }
        _recycledIds.Enqueue(entity.Value);
    }

    private ComponentPool<T> GetOrCreatePool<T>() where T : IComponent
    {
        var type = typeof(T);
        if (!_pools.TryGetValue(type, out var pool))
        {
            pool = new ComponentPool<T>();
            _pools[type] = pool;
        }
        return (ComponentPool<T>)pool;
    }

    public World AddComponent<T>(EntityId entity, T component) where T : IComponent
    {
        GetOrCreatePool<T>().Set(entity, component);
        return this;
    }

    public ref T GetComponent<T>(EntityId entity) where T : IComponent
    {
        return ref GetOrCreatePool<T>().Get(entity);
    }

    public bool HasComponent<T>(EntityId entity) where T : IComponent
    {
        return GetOrCreatePool<T>().Has(entity);
    }

    public bool RemoveComponent<T>(EntityId entity) where T : IComponent
    {
        return GetOrCreatePool<T>().Remove(entity);
    }

    public IReadOnlyDictionary<EntityId, T> Query<T>() where T : IComponent
    {
        return GetOrCreatePool<T>().GetAll();
    }

    public IEnumerable<(EntityId entityId, T1 comp1, T2 comp2)> Query<T1, T2>()
        where T1 : IComponent
        where T2 : IComponent
    {
        var pool1 = GetOrCreatePool<T1>().GetAll();
        foreach (var (entityId, comp1) in pool1)
        {
            if (HasComponent<T2>(entityId))
            {
                yield return (entityId, comp1, GetComponent<T2>(entityId));
            }
        }
    }

    public IEnumerable<(EntityId entityId, T1 comp1, T2 comp2, T3 comp3)> Query<T1, T2, T3>()
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
    {
        var pool1 = GetOrCreatePool<T1>().GetAll();
        foreach (var (entityId, comp1) in pool1)
        {
            if (HasComponent<T2>(entityId) && HasComponent<T3>(entityId))
            {
                yield return (entityId, comp1, GetComponent<T2>(entityId), GetComponent<T3>(entityId));
            }
        }
    }



}

public interface IComponentPool
{
    bool Has(EntityId entity);
    bool Remove(EntityId entity);
}

public class ComponentPool<T> : IComponentPool where T : IComponent
{
    private readonly Dictionary<EntityId, T> _components = new();
    public IReadOnlyDictionary<EntityId, T> GetAll() => _components;

    public void Set(EntityId entity, T component)
    {
        _components[entity] = component;
    }

    public ref T Get(EntityId entity)
    {
        return ref System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrNullRef(_components, entity);
    }

    public bool Has(EntityId entity)
    {
        return _components.ContainsKey(entity);
    }

    public bool Remove(EntityId entity)
    {
        return _components.Remove(entity);
    }
}
