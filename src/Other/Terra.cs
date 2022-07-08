using System;
using Threading = System.Threading;
using System.Collections.Generic;
using Godot;

public class Terra {
    // Declare member variables here. Examples:
    private volatile Octree octree;
    private volatile Node parent;
    private volatile Dictionary<string, MeshInstance> meshes;

    public Position boundries{get; private set;}

    public Terra (Position boundries, Node parent) {
        this.parent = parent;

        this.boundries = boundries;

        octree = new Octree (boundries.GetMax());

        meshes = new Dictionary<string, MeshInstance> ();
    }

    public Octree GetOctree () {
        return octree;
    }

    public OctreeNode TraverseOctree (int posX, int posY, int posZ, int layer) {
        if (CheckBoundries(posX, posY, posZ) && layer < octree.layers) {
                int currentLayer = octree.layers;
                OctreeNode currentNode = octree.mainNode;
                while (currentLayer > layer) {
                    int nodePosX = (int) (posX / (currentLayer * 2));
                    int nodePosY = (int) (posY / (currentLayer * 2));
                    int nodePosZ = (int) (posZ / (currentLayer * 2));

                    currentLayer -= 1;
                    int nodePos = SelectChildOctant (nodePosX, nodePosY, nodePosZ);
                    OctreeNode childNode = currentNode.children[nodePos & 1, (nodePos & 2) >> 1, (nodePos & 4) >> 2];
                    if (!childNode.Initialized)
                        childNode.Initialize ();

                    currentNode = childNode;

                    /* string name = "layer: " + currentLayer + " " + nodePosX * 16 * (float) Math.Pow(2, currentLayer) + " " +
                                   nodePosY * 16 * (float) Math.Pow(2, currentLayer) +
                                   " " + nodePosZ * 16 * (float) Math.Pow(2, currentLayer);
                     if (!meshes.ContainsKey(name))
                     {
                         MeshInstance instance = DebugMesh();
                         instance.Scale = new Vector3(32 * (float) Math.Pow(2, currentLayer - 2),
                             32 * (float) Math.Pow(2, currentLayer - 2),
                             32 * (float) Math.Pow(2, currentLayer - 2));
                         instance.Name = name;
                         instance.Translation = new Vector3(nodePosX * 16 * (float) Math.Pow(2, currentLayer - 1),
                             nodePosY * 16 * (float) Math.Pow(2, currentLayer - 1),
                             nodePosZ * 16 * (float) Math.Pow(2, currentLayer - 1));
                         parent.CallDeferred("add_child", instance);
                         meshes.Add(name, instance);
                     }*/
                }
                if (!currentNode.Initialized)
                    currentNode.Initialize ();

                if (currentLayer == 0) {

                    int pos = SelectChildOctant (posX, posY, posZ);
                    OctreeNode childNode = currentNode.children[pos & 1, (pos & 2) >> 1, (pos & 4) >> 2];

                    return childNode;
                }

                return currentNode;
        }
        return null;
    }

    public bool CheckBoundries(int posX, int posY, int posZ){
        return posX >= 0 && posY >= 0 && posZ >= 0 &&
            posX <= boundries.x * 8 && posY <= boundries.y * 8 && posZ <= boundries.z * 8;
    }

    public void PlaceChunk (int posX, int posY, int posZ, Chunk chunk) {
        OctreeNode node = TraverseOctree (posX, posY, posZ, 0);
        node.chunk = chunk;
     //   node.materialID = (int) (chunk.voxels[0].value);
    }

    public void ReplaceChunk (int posX, int posY, int posZ, Chunk chunk) {
        /*int lolong = (int) Morton3D.encode(posX, posY, posZ);
        OctreeNode node = octree.nodes[0][lolong];
        node.chunk = chunk;
        octree.nodes[0][lolong] = node;*/
    }

    private int SelectChildOctant (int posX, int posY, int posZ) {
        return (posX % 2) * 4 | (posY % 2) * 2 | (posZ % 2);
    }

    private static MeshInstance DebugMesh () {
        SurfaceTool tool = new SurfaceTool ();
        tool.Begin (PrimitiveMesh.PrimitiveType.Lines);

        //Front
        tool.AddVertex (new Vector3 (0, 0, 0));
        tool.AddVertex (new Vector3 (1, 0, 0));
        tool.AddVertex (new Vector3 (1, 0, 0));
        tool.AddVertex (new Vector3 (1, 1, 0));
        tool.AddVertex (new Vector3 (1, 1, 0));
        tool.AddVertex (new Vector3 (0, 1, 0));
        tool.AddVertex (new Vector3 (0, 1, 0));
        tool.AddVertex (new Vector3 (0, 0, 0));

        //Back
        tool.AddVertex (new Vector3 (0, 0, 1));
        tool.AddVertex (new Vector3 (1, 0, 1));
        tool.AddVertex (new Vector3 (1, 0, 1));
        tool.AddVertex (new Vector3 (1, 1, 1));
        tool.AddVertex (new Vector3 (1, 1, 1));
        tool.AddVertex (new Vector3 (0, 1, 1));
        tool.AddVertex (new Vector3 (0, 1, 1));
        tool.AddVertex (new Vector3 (0, 0, 1));

        //BOTTOM
        tool.AddVertex (new Vector3 (0, 0, 0));
        tool.AddVertex (new Vector3 (0, 0, 1));
        tool.AddVertex (new Vector3 (0, 0, 1));
        tool.AddVertex (new Vector3 (1, 0, 1));
        tool.AddVertex (new Vector3 (1, 0, 1));
        tool.AddVertex (new Vector3 (1, 0, 0));
        tool.AddVertex (new Vector3 (1, 0, 0));
        tool.AddVertex (new Vector3 (0, 0, 0));

        //TOP
        tool.AddVertex (new Vector3 (0, 1, 0));
        tool.AddVertex (new Vector3 (0, 1, 1));
        tool.AddVertex (new Vector3 (0, 1, 1));
        tool.AddVertex (new Vector3 (1, 1, 1));
        tool.AddVertex (new Vector3 (1, 1, 1));
        tool.AddVertex (new Vector3 (1, 1, 0));
        tool.AddVertex (new Vector3 (1, 1, 0));
        tool.AddVertex (new Vector3 (0, 1, 0));

        MeshInstance instance = new MeshInstance ();
        instance.Mesh = tool.Commit ();
        return instance;
    }
}