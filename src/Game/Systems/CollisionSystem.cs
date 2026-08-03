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
        foreach (var (e1, t1, b1) in _world.Query<Transform, PhysicsBody>())
        {
            foreach (var (e2, t2, b2) in _world.Query<Transform, PhysicsBody>())
            {
                if (e1 == e2) continue;
                if (b1.Type == PhysicsType.Kinmetic || b2.Type == PhysicsType.Kinmetic) continue;
                if (!EntitiesCollide(t1, t2, b1, b2)) continue;

                if (b1.Shape == ShapeType.Circle && b2.Shape != ShapeType.Circle)
                {
                    ref var tRef = ref _world.GetComponent<Transform>(e1);
                    ref var mRef = ref _world.GetComponent<Movement>(e1);
                    CircleToRec(ref tRef, ref mRef, b1, t2, b2);
                }
                else if (b1.Shape == ShapeType.Circle && b2.Shape == ShapeType.Circle)
                {
                    ref var t1Ref = ref _world.GetComponent<Transform>(e1);
                    ref var m1Ref = ref _world.GetComponent<Movement>(e1);
                    ref var t2Ref = ref _world.GetComponent<Transform>(e2);
                    ref var m2Ref = ref _world.GetComponent<Movement>(e2);
                    CircleToCircle(ref t1Ref, ref m1Ref, b1, ref t2Ref, ref m2Ref, b2);
                }
            }
        }
    }

    private void CircleToCircle(ref Transform t1, ref Movement m1, PhysicsBody b1, ref Transform t2, ref Movement m2, PhysicsBody b2)
    {
        float radius1 = b1.Width / 2;
        Vector2 pos1 = b1.GetCirPos(t1);
        float radius2 = b2.Width / 2;
        Vector2 pos2 = b2.GetCirPos(t2);

        Vector2 delta = pos1 - pos2;
        float dis = delta.Length();
        float minDis = radius1 + radius2;

        Vector2 normal = dis > 0.0001f ? Vector2.Normalize(delta) : new Vector2(0, -1);

        float overlap = minDis - dis;
        if (overlap > 0)
        {
            t1.Pos += normal * (overlap * 0.5f);
            t2.Pos -= normal * (overlap * 0.5f);
        }

        Vector2 tempDir = m1.Dir;
        m1.Dir = Vector2.Reflect(m1.Dir, normal);
        m2.Dir = Vector2.Reflect(m2.Dir, -normal);
    }

    private void CircleToRec(ref Transform cirT, ref Movement cirM, PhysicsBody cirB, Transform recT, PhysicsBody recB)
    {
        Vector2 cirPos = cirB.GetCirPos(cirT);
        float radius = cirB.Width / 2f;
        Rectangle rec = recB.GetRec(recT);

        // Nächsten Punkt ermitteln, um den Normalenvektor für den Abprallwinkel zu berechnen
        float closestX = Math.Clamp(cirPos.X, rec.X, rec.X + rec.Width);
        float closestY = Math.Clamp(cirPos.Y, rec.Y, rec.Y + rec.Height);
        Vector2 closestPoint = new(closestX, closestY);

        Vector2 delta = cirPos - closestPoint;
        float dis = delta.Length();

        Vector2 normal = dis > 0.0001f ? Vector2.Normalize(delta) : new Vector2(0, -1);

        // 1. Richtung abprallen lassen
        cirM.Dir = Vector2.Reflect(cirM.Dir, normal);

        // 2. Kugel aus dem Rechteck drängen
        float overlap = radius - dis;
        if (overlap > 0)
        {
            cirT.Pos += normal * overlap;
        }
    }
}
