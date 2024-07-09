using Godot;
using System;

public struct RawChunk
{
    public Godot.Collections.Array[] arrays { get; set; }
    public StandardMaterial3D[] materials { get; set; }
    public Vector3[][] colliderFaces { get; set; }
}