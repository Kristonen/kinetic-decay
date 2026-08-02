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
        foreach (var (_, transform, cir) in _world.Query<Transform, Circle>())
        {
            RL.DrawCircleV(transform.Pos, cir.Radius, cir.Color);
        }

        foreach(var (_, transform, rec) in _world.Query<Transform, Rec>()){
            RL.DrawRectangleRec(GetRectangle(transform, rec), rec.Color);
        }
    }

    private Rectangle GetRectangle(Transform transform, Rec rec) => new(transform.Pos.X, transform.Pos.Y, rec.Width, rec.Height);
}
