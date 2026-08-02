using Core.Ecs;

public class UpdatePipeline
{
    private readonly List<AbstractUpdateSystem> _systems = new();

    public void Add(AbstractUpdateSystem system) => _systems.Add(system);

    public void Update(float dt)
    {
        for (int i = 0; i < _systems.Count; i++)
        {
            _systems[i].Update(dt);
        }
    }
}

public abstract class AbstractUpdateSystem
{
    protected World _world;
    public AbstractUpdateSystem(World world)
    {
        _world = world;
    }

    public abstract void Update(float dt);
}

public class MovementSystem : AbstractUpdateSystem
{

    public MovementSystem(World world) : base(world){}

    public override void Update(float dt)
    {
        var moveEntities = _world.Query<Movement>();

        foreach (var (entity, move) in moveEntities)
        {
            if (_world.HasComponent<Transform>(entity))
            {
                ref var transform = ref _world.GetComponent<Transform>(entity);
                transform.Pos += move.Dir * move.Speed * dt;
            }
        }
    }
}
