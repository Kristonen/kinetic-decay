using System.Numerics;
using Core.Ecs;

public struct Movement : IComponent{
    public Vector2 Dir;
    public float Speed;

    public Movement(Vector2 dir)
    {
        Dir = dir;
        Speed = 0;
    }

    public Movement(float speed)
    {
        Dir = new(0, 0);
        Speed = speed;
    }

    public Movement(Vector2 dir, float speed)
    {
        Dir = dir;
        Speed = speed;
    }
}
