using Godot;

public partial class PrimitiveResources
{
    public static void Register(Registry registry)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        Texture2D nativeTexture = (Texture2D)GD.Load("res://assets/textures/blocks/NorthenForestDirt256px.png");
        material.AlbedoTexture = nativeTexture;
        TerraObject dirt = new TerraObject("dirt", material);
        registry.RegisterObject(dirt);

        material = new StandardMaterial3D();
        nativeTexture = (Texture2D)GD.Load("res://assets/textures/blocks/NorthenForestGrass256px.png");
        material.AlbedoTexture = nativeTexture;
        TerraObject grass = new TerraObject("grass", material);
        registry.RegisterObject(grass);
    }
}
