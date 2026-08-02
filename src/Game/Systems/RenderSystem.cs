using Core.Ecs;
using Raylib_cs;
using RL = Raylib_cs.Raylib;

public class RenderPipeline()
{
    private readonly List<AbstractRenderSystem> _systems = new();
    public void Add(AbstractRenderSystem system) => _systems.Add(system);

    public void Draw(){
        for (int i = 0; i < _systems.Count; i++)
        {
            _systems[i].Draw();
        }
    }
}

public abstract class AbstractRenderSystem
{
    protected World _world;
    public AbstractRenderSystem(World world)
    {
        _world = world;
    }

    public abstract void Draw();
}

public class RenderSystem : AbstractRenderSystem
{
    public RenderSystem(World world) : base(world){}

    public override void Draw()
    {
        var circleEntities = _world.Query<Circle>();

        foreach (var (entity, circle) in circleEntities)
        {
            if (_world.HasComponent<Transform>(entity))
            {
                ref var transform = ref _world.GetComponent<Transform>(entity);
                RL.DrawCircleV(transform.Pos, circle.Radius, circle.Color);
            }
        }

        var recEntities = _world.Query<Rec>();

        foreach (var (entity, rec) in recEntities)
        {
            if (_world.HasComponent<Transform>(entity))
            {
                ref var transform = ref _world.GetComponent<Transform>(entity);
                RL.DrawRectangleRec(GetRectangle(transform, rec), rec.Color);
            }
        }
    }

    private Rectangle GetRectangle(Transform transform, Rec rec) => new(transform.Pos.X, transform.Pos.Y, rec.Width, rec.Height);
}
