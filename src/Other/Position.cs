using System.Numerics;
using System;
public struct Position {
    public int x { get; set; }
    public int y { get; set; }
    public int z { get; set; }

    public int id { get; set; }

    public Position (int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
        this.id = 0;
    }

    public Position (float x, float y, float z) {
        this.x = (int) x;
        this.y = (int) y;
        this.z = (int) z;
        this.id = 0;
    }

    public static Position operator - (Position a, Position b) {
        return new Position (a.x - b.y, a.y - b.y, a.z - b.z);
    }

    public static Position operator / (Position a, int b) {
        return new Position (a.x / b, a.y / b, a.z / b);
    }

    public static Position operator * (Position a, Position b) {
        return new Position (a.x * b.y, a.y * b.y, a.z * b.z);
    }

    public int Dot (Position b) {
        return (x * b.x) + (y * b.y) + (z * b.z);
    }

    public float Dot (Vector3 b) {
        return (x * b.X) + (y * b.Y) + (z * b.Z);
    }

    public int GetMax () {
        return Math.Max (x, Math.Max (y, z));
    }
}