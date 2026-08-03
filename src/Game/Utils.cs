using System.Numerics;

public static class Utils{
    private static readonly Random _rng = new();

    public static Vector2 GetRandomDirection(){
        float angle = (float)(_rng.NextDouble() * Math.PI * 2);
        float x = MathF.Cos(angle);
        float y = MathF.Sin(angle);
        return new(x, y);
    }
}
