using System.Numerics;
using Core.Ecs;
using Raylib_cs;

public struct Transform : IComponent
{
    public Vector2 Pos;
    public int Rotation;
    public Vector2 Scale;
}

public struct Circle : IComponent
{
    public float Radius;
    public Color Color;
}

public struct Rec : IComponent{
    public float Width, Height;
    public Color Color;

    public Rec(float width, float height, Color color)
    {
        Width = width;
        Height = height;
        Color = color;
    }

    public Rec(float width, float height)
    {
        Width = width;
        Height = height;
    }
}
