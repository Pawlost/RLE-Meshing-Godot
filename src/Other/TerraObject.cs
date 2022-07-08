using Godot;
using System;

public class TerraObject
{
    public int worldID { get; set; }
    public string fullName { get; set; }
    public string name { get; }
    public SpatialMaterial material { get; }
    public TerraMesh mesh { get; set; }

    public TerraObject(string name, SpatialMaterial material)
    {
        this.name = name;
        this.material = material;
    }
}