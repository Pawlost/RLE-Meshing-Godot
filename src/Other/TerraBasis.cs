using System;
public struct TerraBasis {

    public TerraVector3[] matrix;

    public static TerraBasis InitEmpty () {

        TerraVector3[] matrix = new TerraVector3[3];

        matrix[0].x = 1;
        matrix[0].y = 0;
        matrix[0].z = 0;
        matrix[1].x = 0;
        matrix[1].y = 1;
        matrix[1].z = 0;
        matrix[2].x = 0;
        matrix[2].y = 0;
        matrix[2].z = 1;

        return new TerraBasis (matrix);
    }

    public TerraBasis (float xx, float xy, float xz, float yx, float yy, float yz, float zx, float zy, float zz) {
        matrix = new TerraVector3[3];

        matrix[0].x = xx;
        matrix[0].y = xy;
        matrix[0].z = xz;
        matrix[1].x = yx;
        matrix[1].y = yy;
        matrix[1].z = yz;
        matrix[2].x = zx;
        matrix[2].y = zy;
        matrix[2].z = zz;
    }

    public TerraBasis (TerraVector3 axis, float radians) {

        matrix = new TerraVector3[3];

        TerraVector3 axisSQ = new TerraVector3 (axis.x * axis.x, axis.y * axis.y, axis.z * axis.z);

        float cosine = (float) Math.Cos (radians);
        float sine = (float) Math.Sin (radians);

        matrix[0].x = axisSQ.x + cosine * (1.0f - axisSQ.x);
        matrix[0].y = axis.x * axis.y * (1.0f - cosine) - axis.z * sine;
        matrix[0].z = axis.z * axis.x * (1.0f - cosine) + axis.y * sine;

        matrix[1].x = axis.x * axis.y * (1.0f - cosine) + axis.z * sine;
        matrix[1].y = axisSQ.y + cosine * (1.0f - axisSQ.y);
        matrix[1].z = axis.y * axis.z * (1.0f - cosine) - axis.x * sine;

        matrix[2].x = axis.z * axis.x * (1.0f - cosine) - axis.y * sine;
        matrix[2].y = axis.y * axis.z * (1.0f - cosine) + axis.x * sine;
        matrix[2].z = axisSQ.z + cosine * (1.0f - axisSQ.z);
    }

    public TerraBasis (TerraVector3[] matrix) {
        this.matrix = matrix;
    }

    public TerraBasis (TerraVector3 euler) {
        matrix = new TerraVector3[3];

        float c, s;

        c = (float) Math.Cos (euler.x);
        s = (float) Math.Sin (euler.x);
        TerraBasis xmat = new TerraBasis (1.0f, 0.0f, 0.0f, 0.0f, c, -s, 0.0f, s, c);

        c = (float) Math.Cos (euler.y);
        s = (float) Math.Sin (euler.y);
        TerraBasis ymat = new TerraBasis (c, 0.0f, s, 0.0f, 1.0f, 0.0f, -s, 0.0f, c);

        c = (float) Math.Cos (euler.z);
        s = (float) Math.Sin (euler.z);
        TerraBasis zmat = new TerraBasis (c, -s, 0.0f, s, c, 0.0f, 0.0f, 0.0f, 1.0f);

        //optimizer will optimize away all this anyway
        this = ymat * xmat * zmat;
    }

    public static TerraBasis operator * (TerraBasis matrixA, TerraBasis matrixB) {
        return new TerraBasis (matrixA.Tdotx (matrixB.matrix[0]), matrixA.Tdoty (matrixB.matrix[0]), matrixA.Tdotz (matrixB.matrix[0]),
            matrixA.Tdotx (matrixB.matrix[1]), matrixA.Tdoty (matrixB.matrix[1]), matrixA.Tdotz (matrixB.matrix[1]),
            matrixA.Tdotx (matrixB.matrix[2]), matrixA.Tdoty (matrixB.matrix[2]), matrixA.Tdotz (matrixB.matrix[2]));
    }

    public float Tdotx (TerraVector3 v) {
        return matrix[0].x * v.x + matrix[1].x * v.y + matrix[2].x * v.z;
    }
    public float Tdoty (TerraVector3 v) {
        return matrix[0].y * v.x + matrix[1].y * v.y + matrix[2].y * v.z;
    }
    public float Tdotz (TerraVector3 v) {
        return matrix[0].z * v.x + matrix[1].z * v.y + matrix[2].z * v.z;
    }
}