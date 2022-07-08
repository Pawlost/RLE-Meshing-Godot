using System;
using System.Numerics;
public class OctreeNode {
    const int DIM_LEN = 2;
    public Vector3 center { get; private set; }
    public int size { get; private set; }
    public bool Initialized { get; private set; }
    public OctreeNode[, , ] children { get; private set; }
    public Chunk chunk { get; set; }
    public static int numNodes = 0;
    public static int numNodesInit = 0;
    public int layer { get; private set; }

    public OctreeNode (int size, int layer) {
        this.size = size;
        this.center = center;
        this.layer = layer;
        this.Initialized = false;
        numNodes++;
    }

    public void Initialize () {
        children = new OctreeNode[2, 2, 2];
        for (int i = 0; i < DIM_LEN; i++)
            for (int j = 0; j < DIM_LEN; j++)
                for (int k = 0; k < DIM_LEN; k++) {
                    children[i, j, k] = new OctreeNode (size / 2, layer - 1);
                }
        Initialized = true;
        numNodesInit++;
    }

    public OctreeNode SelectChild (float x, float y, float z) {
        return children[Convert.ToInt32 (x > center.X), Convert.ToInt32 (y > center.Y), Convert.ToInt32 (z > center.Z)];
    }
}