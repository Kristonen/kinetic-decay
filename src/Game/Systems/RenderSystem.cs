using Core.Ecs;
using Raylib_cs;
using RL = Raylib_cs.Raylib;

public class RenderSystem{

    private World _world;

    public RenderSystem(World world){
        _world = world;
    }

    public void Draw() {
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

        foreach (var (entity, rec) in recEntities) {
            if (_world.HasComponent<Transform>(entity)) {
                ref var transform = ref _world.GetComponent<Transform>(entity);
                RL.DrawRectangleRec(GetRectangle(transform, rec), rec.Color);
            }
        }
    }

    private Rectangle GetRectangle(Transform transform, Rec rec) => new(transform.Pos.X, transform.Pos.Y, rec.Width, rec.Height);
}
