namespace Zafiro.Avalonia.Graphs.Tests.Core;

public struct Vector2D
{
    public double X { get; set; }
    public double Y { get; set; }

    public Vector2D(double x, double y)
    {
        X = x;
        Y = y;
    }

    public static Vector2D operator +(Vector2D a, Vector2D b)
    {
        return new Vector2D(a.X + b.X, a.Y + b.Y);
    }

    public static Vector2D operator -(Vector2D a, Vector2D b)
    {
        return new Vector2D(a.X - b.X, a.Y - b.Y);
    }

    public static Vector2D operator *(Vector2D v, double scalar)
    {
        return new Vector2D(v.X * scalar, v.Y * scalar);
    }

    public static Vector2D operator /(Vector2D v, double scalar)
    {
        return new Vector2D(v.X / scalar, v.Y / scalar);
    }

    public double Magnitude()
    {
        return Math.Sqrt(X * X + Y * Y);
    }

    public Vector2D Normalize()
    {
        var magnitude = Magnitude();
        return new Vector2D(X / magnitude, Y / magnitude);
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}