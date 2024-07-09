using Godot;
using System;

public struct Position
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public int id { get; set; }

    public Position(int x, int y, int z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.id = 0;
    }

    public Position(float x, float y, float z)
    {
        this.X = (int)x;
        this.Y = (int)y;
        this.Z = (int)z;
        this.id = 0;
    }

    public static Position operator -(Position a, Position b)
    {
        return new Position(a.X - b.Y, a.Y - b.Y, a.Z - b.Z);
    }

    public static Position operator /(Position a, int b)
    {
        return new Position(a.X / b, a.Y / b, a.Z / b);
    }

    public static Position operator *(Position a, Position b)
    {
        return new Position(a.X * b.Y, a.X * b.Y, a.X * b.Y);
    }

    public int Dot(Position b)
    {
        return (X * b.X) + (Y * b.Y) + (Z * b.Z);
    }

    public float Dot(Vector3 b)
    {
        return (X * b.X) + (Y * b.Y) + (Z * b.Z);
    }

    public int GetMax()
    {
        return Math.Max(X, Math.Max(Y, Z));
    }
}
