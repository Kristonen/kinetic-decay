using System.Numerics;

public enum ShapeType
{
    Circle, Rectangle
}

public enum PhysicsType
{
    Dynamic, Static, Kinmetic
}

public struct PhysicsBody
{
    public ShapeType Shape;
    public PhysicsType Type;
    public Vector2 Offset;
    public float Width;
    public float Height;
    public float Restitution;

    public static PhysicsBody CreateCircleBody(float radius, float restitution, PhysicsType type = PhysicsType.Dynamic)
    {
        return new()
        {
            Shape = ShapeType.Circle,
            Width = radius * 2,
            Height = radius * 2,
            Restitution = restitution,
            Type = type,
        };
    }

    public static PhysicsBody CreateRecBody(float width, float height, float restitution, PhysicsType type = PhysicsType.Dynamic)
    {
        return new()
        {
            Shape = ShapeType.Circle,
            Width = width,
            Height = height,
            Restitution = restitution,
            Type = type,
        };
    }
}
