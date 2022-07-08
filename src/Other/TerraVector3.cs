public struct TerraVector3 {
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    public TerraVector3 (int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public TerraVector3 (float x, float y, float z) {
        this.x = (int) x;
        this.y = (int) y;
        this.z = (int) z;
    }
    public static TerraVector3 operator + (TerraVector3 a, TerraVector3 b) {
        return new TerraVector3 (a.x + b.y, a.y + b.y, a.z + b.z);
    }

    public static TerraVector3 operator - (TerraVector3 a, TerraVector3 b) {
        return new TerraVector3 (a.x - b.y, a.y - b.y, a.z - b.z);
    }

    public static TerraVector3 operator / (TerraVector3 a, int b) {
        return new TerraVector3 (a.x / b, a.y / b, a.z / b);
    }

    public static TerraVector3 operator * (TerraVector3 a, TerraVector3 b) {
        return new TerraVector3 (a.x * b.y, a.y * b.y, a.z * b.z);
    }

    public static bool operator == (TerraVector3 a, TerraVector3 b) {
        return a.x == b.x && b.y == b.y && a.z == b.z;
    }

    public static bool operator != (TerraVector3 a, TerraVector3 b) {
        return a.x != b.x && b.y != b.y && a.z != b.z;
    }

    public float Dot (Position b) {
        return (x * b.x) + (y * b.y) + (z * b.z);
    }

    public float Dot (TerraVector3 b) {
        return (x * b.x) + (y * b.y) + (z * b.z);
    }
}