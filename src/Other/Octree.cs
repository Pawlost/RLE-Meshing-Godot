using System;
public class Octree {
    public int layers { get; set; }
    public int size { get; set; }
    public OctreeNode mainNode { get; set; }

    public Octree (int size) {
        this.size = size;
        // Calculate number of layers needed to represent the level
        layers = (int) Math.Log (Math.Pow (size, 3), 2);

        // Generate all the OctreeNodes
        mainNode = new OctreeNode (size, layers);
        mainNode.Initialize ();
    }
}