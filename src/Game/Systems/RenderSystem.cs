using System.Numerics;
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
    public RenderSystem(World world) : base(world) { }

    public override void Draw()
    {
        foreach (var (_, transform, cir) in _world.Query<Transform, Circle>())
        {
            RL.DrawCircleV(transform.Pos, cir.Radius, cir.Color);
        }

        foreach (var (_, transform, rec) in _world.Query<Transform, Rec>())
        {
            RL.DrawRectangleRec(rec.GetRec(transform.Pos), rec.Color);
        }
    }
}

public class HelperRenderSystem : AbstractRenderSystem
{
    public HelperRenderSystem(World world) : base(world) { }

    public override void Draw()
    {
        if (!Game.GameState.Game.Helper) return;

        foreach (var (_, transform, body) in _world.Query<Transform, PhysicsBody>())
        {
            if (body.Shape == ShapeType.Circle)
            {
                RL.DrawCircleV(transform.Pos + body.Offset, body.Width / 2, new(200, 25, 200, 100));
            }
            else
            {
                RL.DrawRectangleRec(body.GetRec(transform), new(200, 25, 200, 100));
            }
        }
    }
}
