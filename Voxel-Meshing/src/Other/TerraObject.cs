using Godot;

public partial class TerraObject
{
    public int worldID { get; set; }
    public string fullName { get; set; }
    public string name { get; }
    public StandardMaterial3D material { get; }

    public TerraObject(string name, StandardMaterial3D material)
    {
        this.name = name;
        this.material = material;
    }
}


