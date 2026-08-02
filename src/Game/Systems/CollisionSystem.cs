using System.Numerics;
using Core.Ecs;
using Raylib_cs;
using RL = Raylib_cs.Raylib;

public class CollisionPipeLine
{
    private readonly List<AbstractCollisionSystem> _systems = new();

    public void Add(AbstractCollisionSystem system) => _systems.Add(system);

    public void Collision(float dt)
    {
        for (int i = 0; i < _systems.Count; i++)
        {
            _systems[i].Collision(dt);
        }
    }
}

public abstract class AbstractCollisionSystem
{
    protected World _world;

    public AbstractCollisionSystem(World world)
    {
        _world = world;
    }

    public abstract void Collision(float dt);

    protected bool EntitiesCollide(Transform t1, Transform t2, PhysicsBody b1, PhysicsBody b2)
    {
        // Cir vs Cir
        if (b1.Shape == ShapeType.Circle && b2.Shape == ShapeType.Circle)
        {
            return RL.CheckCollisionCircles(b1.GetCirPos(t1), b1.Width / 2, b2.GetCirPos(t2), b2.Width / 2);
        }
        // Cir vs Rec
        else if (b1.Shape == ShapeType.Circle && b2.Shape == ShapeType.Rectangle)
        {
            return RL.CheckCollisionCircleRec(b1.GetCirPos(t1), b1.Width / 2, b2.GetRec(t2));
        }
        // Rec vs Cir
        else if (b1.Shape == ShapeType.Rectangle && b2.Shape == ShapeType.Circle)
        {
            return RL.CheckCollisionCircleRec(b2.GetCirPos(t2), b2.Width / 2, b1.GetRec(t1));
        }
        // Rec vs Rec
        else
        {
            return RL.CheckCollisionRecs(b1.GetRec(t1), b2.GetRec(t2));
        }
    }
}

public class SimpleCollision : AbstractCollisionSystem
{
    public SimpleCollision(World world) : base(world) { }

    public override void Collision(float dt)
    {
        foreach(var (e1, t1, b1) in _world.Query<Transform, PhysicsBody>())
        {
            foreach(var (e2, t2, b2) in _world.Query<Transform, PhysicsBody>()){
                if (e1 == e2) continue;
                if (EntitiesCollide(t1, t2, b1, b2))
                {
                    Console.WriteLine("COLLISION!");
                }
            }
        }
    }
}
