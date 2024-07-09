using Godot;

namespace Godotmeshing.src.Other
{
    public partial class Converter
    {
        public static TerraVector3 ConvertVector(Vector3 godotOrigin)
        {
            return new TerraVector3(godotOrigin.X, godotOrigin.Y, godotOrigin.Z);
        }

        public static TerraBasis ConvertBasis(Basis godotBasis)
        {
            TerraBasis basis = TerraBasis.InitEmpty();
            basis.matrix[0] = new TerraVector3(godotBasis[0].X, godotBasis[0].Y, godotBasis[0].Z);
            basis.matrix[1] = new TerraVector3(godotBasis[1].X, godotBasis[1].Y, godotBasis[1].Z);
            basis.matrix[2] = new TerraVector3(godotBasis[2].X, godotBasis[2].Y, godotBasis[2].Z);
            return basis;
        }
    }
}


