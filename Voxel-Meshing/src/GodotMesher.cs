using Godot;
using Godot.Collections;
using Godotmeshing.src.Other;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using static Godot.HttpClient;

public partial class GodotMesher
{
    private volatile Registry reg;

    public void SetRegistry(Registry reg)
    {
        this.reg = reg;
    }

    public MeshInstance3D MeshChunk(Chunk chunk, ArrayPool<Position> pool)
    {

        RawChunk rawChunk = new RawChunk();

        rawChunk.arrays = new Godot.Collections.Array[chunk.Materials - 1];
        rawChunk.materials = new StandardMaterial3D[chunk.Materials - 1];
        rawChunk.colliderFaces = new Vector3[chunk.Materials - 1][];

        if (chunk.Materials > 1)
        {
            rawChunk = Meshing(chunk, rawChunk, pool);
        }
        else
        {
            rawChunk = FastGodotCube(chunk, rawChunk);
        }

        ArrayMesh mesh = new ArrayMesh();

        // RenderingServer.MeshCreate();

        mesh.GetRid();


       // Rid body = PhysicsServer3D.BodyCreate();

        for (int t = 0; t < rawChunk.arrays.Count(); t++)
        {
            StandardMaterial3D material = rawChunk.materials[t];

            Godot.Collections.Array godotArraY = rawChunk.arrays[t];

            if (godotArraY.Count > 0)
            {

                //Rid shape = PhysicsServer3D.CustomShapeCreate();
                //    PhysicsServer3D.ShapeSetData (shape, godotArraY);
                //  PhysicsServer3D.BodyAddShape (body, shape, new Transform3D (Basis.Identity, new Vector3 (chunk.x, chunk.y, chunk.z)));
                
                mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, godotArraY);
                mesh.SurfaceSetMaterial(mesh.GetSurfaceCount() - 1, material);

            //      RenderingServer.MeshAddSurfaceFromArrays(meshID, RenderingServer.PrimitiveType.Triangles, godotArraY);
            //     RenderingServer.MeshSurfaceSetMaterial(meshID, RenderingServer.MeshGetSurfaceCount(meshID) - 1, material.GetRid());
            }
        }
        var meshInstance = new MeshInstance3D();
        meshInstance.Mesh = mesh;
        meshInstance.Position = new Vector3(chunk.x, chunk.y, chunk.z);


        return meshInstance;

        //    RenderingServer.InstanceSetBase(instance, meshID);
        //  RenderingServer.InstanceSetTransform(instance, new Transform3D(Basis.Identity, new Vector3(chunk.x, chunk.y, chunk.z)));
        //  RenderingServer.InstanceSetScenario(meshInstance.GetRid(), ScenarioId);
        //   PhYsicsServer.BodYSetSpace (bodY, GetWorld ().Space);
    }

    private RawChunk Meshing(Chunk chunk, RawChunk rawChunk, ArrayPool<Position> pool)
    {
        Position[] values = MeshingUtils.NaiveGreedyMeshing(chunk, pool);
        Queue<Position>[][] stacks = new Queue<Position>[6][];
        int[] siZe = new int[chunk.Materials - 1];

        for (int side = 0; side < 6; side++)
        {
            stacks[side] = new Queue<Position>[chunk.Materials - 1];
            for (int t = 0; t < chunk.Materials - 1; t++)
            {
                stacks[side][t] = new Queue<Position>(Constants.CHUNK_SIZE3D);
            }

            MeshingUtils.GreedyMeshing(values, side, stacks[side]);
            for (int t = 0; t < chunk.Materials - 1; t++)
            {
                siZe[t] += stacks[side][t].Count;
            }
        }

        pool.Return(values);

        for (int t = 0; t < chunk.Materials - 1; t++)
        {
            StandardMaterial3D material = reg.SelectByID(t + 1).material;
            Vector3[] vertice = new Vector3[siZe[t]];
            int[] indices = new int[siZe[t] + (siZe[t] / 2)];
            Vector3[] normals = new Vector3[siZe[t]];
            Vector2[] uvs = new Vector2[siZe[t]];
            float teXtureWidth = 2048f / material.AlbedoTexture.GetWidth();
            float teXtureHeight = 2048f / material.AlbedoTexture.GetHeight();

            if (siZe[t] > 0)
            {
                int indeX = 0;
                int i = 0;
                for (int side = 0; side < 6; side++)
                {
                    for (; i < siZe[t]; i += 4)
                    {
                        if (stacks[side][t].Count > 0)
                        {

                            indices[indeX] = i;
                            indices[indeX + 1] = i + 1;
                            indices[indeX + 2] = i + 2;
                            indices[indeX + 3] = i + 2;
                            indices[indeX + 4] = i + 3;
                            indices[indeX + 5] = i;
                            indeX += 6;

                            for (int s = 0; s < 4; s++)
                            {
                                Position position = stacks[side][t].Dequeue();

                                Vector3 vector = new Vector3();
                                vector.X = position.X * Constants.VOXEL_SIZE;
                                vector.Y = position.Y * Constants.VOXEL_SIZE;
                                vector.Z = position.Z * Constants.VOXEL_SIZE;
                                vertice[i + s] = vector;

                                Vector2 uv = new Vector2();
                                Vector3 normal = new Vector3();
                                switch (side)
                                {
                                    case 0:
                                        //Front
                                        normal.X = 0f;
                                        normal.Y = 0f;
                                        normal.Z = -1f;
                                        normals[i + s] = normal;

                                        uv.X = vector.X * teXtureWidth;
                                        uv.Y = vector.Y * teXtureHeight;
                                        uvs[i + s] = uv;
                                        break;
                                    case 1:
                                        //Back
                                        normal.X = 0f;
                                        normal.Y = 0f;
                                        normal.Z = 1f;
                                        normals[i + s] = normal;

                                        uv.X = vector.X * teXtureWidth;
                                        uv.Y = vector.Y * teXtureHeight;
                                        uvs[i + s] = uv;

                                        break;
                                    case 2:
                                        //Right
                                        normal.X = -1f;
                                        normal.Y = 0f;
                                        normal.Z = 0f;
                                        normals[i + s] = normal;

                                        uv.X = vector.Z * teXtureWidth;
                                        uv.Y = vector.Y * teXtureHeight;
                                        uvs[i + s] = uv;
                                        break;
                                    case 3:
                                        //Left
                                        normal.X = 1f;
                                        normal.Y = 0f;
                                        normal.Z = 0f;
                                        normals[i + s] = normal;

                                        uv.X = vector.Z * teXtureWidth;
                                        uv.Y = vector.Y * teXtureHeight;
                                        uvs[i + s] = uv;
                                        break;
                                    case 4:
                                        //Top
                                        normal.X = 0f;
                                        normal.Y = 1f;
                                        normal.Z = 0f;
                                        normals[i + s] = normal;

                                        uv.X = vector.X * teXtureWidth;
                                        uv.Y = vector.Z * teXtureHeight;
                                        uvs[i + s] = uv;
                                        break;
                                    case 5:
                                        //Bottom
                                        normal.X = 0f;
                                        normal.Y = 0f;
                                        normal.Z = 0f;
                                        normals[i + s] = normal;

                                        uv.X = vector.X * teXtureWidth;
                                        uv.Y = vector.Z * teXtureHeight;
                                        uvs[i + s] = uv;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            Godot.Collections.Array godotArraY = new Godot.Collections.Array();
            if (vertice.Length > 0)
            {
                godotArraY.Resize((int)RenderingServer.ArrayType.Max);

                godotArraY[(int)RenderingServer.ArrayType.Vertex] = vertice;
                godotArraY[(int)RenderingServer.ArrayType.Normal] = normals;
                godotArraY[(int)RenderingServer.ArrayType.TexUV] = uvs;
                godotArraY[(int)RenderingServer.ArrayType.Index] = indices;
            }
            rawChunk.arrays[t] = godotArraY;
            rawChunk.materials[t] = material;
        }
        return rawChunk;
    }

    public RawChunk FastGodotCube(Chunk chunk, RawChunk rawChunk)
    {
        Godot.Collections.Array godotArraY = new Godot.Collections.Array();
        godotArraY.Resize((int)RenderingServer.ArrayType.Max);
        rawChunk.arrays = new Godot.Collections.Array[1];
        rawChunk.materials = new StandardMaterial3D[1];
        rawChunk.colliderFaces = new Vector3[1][];

        int objectID = chunk.Voxels[0].value;

        StandardMaterial3D material = reg.SelectByID(objectID).material;

        float teXtureWidth = 2048f / material.AlbedoTexture.GetWidth();
        float teXtureHeight = 2048f / material.AlbedoTexture.GetHeight();

        Vector3[] vertice = new Vector3[24];
        Vector3[] normals = new Vector3[24];
        Vector2[] uvs = new Vector2[24];

        //FRONT
        vertice[0] = new Vector3(0, 0, 0);
        vertice[1] = new Vector3(Constants.CHUNK_LENGHT, 0, 0);
        vertice[2] = new Vector3(Constants.CHUNK_LENGHT, Constants.CHUNK_LENGHT, 0);
        vertice[3] = new Vector3(0, Constants.CHUNK_LENGHT, 0);

        for (int i = 0; i < 4; i++)
        {
            normals[i] = new Vector3(0, 0, -1);
            uvs[i].X = vertice[i].X * teXtureWidth;
            uvs[i].Y = vertice[i].Y * teXtureHeight;
        }

        //BACK
        vertice[4] = new Vector3(Constants.CHUNK_LENGHT, 0, Constants.CHUNK_LENGHT);
        vertice[5] = new Vector3(0, 0, Constants.CHUNK_LENGHT);
        vertice[6] = new Vector3(0, Constants.CHUNK_LENGHT, Constants.CHUNK_LENGHT);
        vertice[7] = new Vector3(Constants.CHUNK_LENGHT, Constants.CHUNK_LENGHT, Constants.CHUNK_LENGHT);

        for (int i = 4; i < 8; i++)
        {
            normals[i] = new Vector3(0, 0, 1);
            uvs[i].X = vertice[i].X * teXtureWidth;
            uvs[i].Y = vertice[i].Y * teXtureHeight;
        }

        //LEFT
        vertice[8] = new Vector3(0, 0, Constants.CHUNK_LENGHT);
        vertice[9] = new Vector3(0, 0, 0);
        vertice[10] = new Vector3(0, Constants.CHUNK_LENGHT, 0);
        vertice[11] = new Vector3(0, Constants.CHUNK_LENGHT, Constants.CHUNK_LENGHT);

        for (int i = 8; i < 12; i++)
        {
            normals[i] = new Vector3(1, 0, 0);
            uvs[i].X = vertice[i].Z * teXtureWidth;
            uvs[i].Y = vertice[i].Y * teXtureHeight;
        }

        //RIGHT
        vertice[12] = new Vector3(Constants.CHUNK_LENGHT, 0, 0);
        vertice[13] = new Vector3(Constants.CHUNK_LENGHT, 0, Constants.CHUNK_LENGHT);
        vertice[14] = new Vector3(Constants.CHUNK_LENGHT, Constants.CHUNK_LENGHT, Constants.CHUNK_LENGHT);
        vertice[15] = new Vector3(Constants.CHUNK_LENGHT, Constants.CHUNK_LENGHT, 0);

        for (int i = 12; i < 16; i++)
        {
            normals[i] = new Vector3(-1, 0, 0);
            uvs[i].X = vertice[i].Z * teXtureWidth;
            uvs[i].Y = vertice[i].Y * teXtureHeight;
        }

        // TOP
        vertice[16] = new Vector3(0, Constants.CHUNK_LENGHT, 0);
        vertice[17] = new Vector3(Constants.CHUNK_LENGHT, Constants.CHUNK_LENGHT, 0);
        vertice[18] = new Vector3(Constants.CHUNK_LENGHT, Constants.CHUNK_LENGHT, Constants.CHUNK_LENGHT);
        vertice[19] = new Vector3(0, Constants.CHUNK_LENGHT, Constants.CHUNK_LENGHT);

        for (int i = 16; i < 20; i++)
        {
            normals[i] = new Vector3(0, 1, 0);
            uvs[i].X = vertice[i].X * teXtureWidth;
            uvs[i].Y = vertice[i].Z * teXtureHeight;
        }

        //BOTTOM
        vertice[20] = new Vector3(0, 0, Constants.CHUNK_LENGHT);
        vertice[21] = new Vector3(Constants.CHUNK_LENGHT, 0, Constants.CHUNK_LENGHT);
        vertice[22] = new Vector3(Constants.CHUNK_LENGHT, 0, 0);
        vertice[23] = new Vector3(0, 0, 0);

        for (int i = 20; i < 24; i++)
        {
            normals[i] = new Vector3(0, -1, 0);
            uvs[i].X = vertice[i].X * teXtureWidth;
            uvs[i].Y = vertice[i].Z * teXtureHeight;
        }

        godotArraY[(int)RenderingServer.ArrayType.Vertex] = vertice;
        godotArraY[(int)RenderingServer.ArrayType.Normal] = normals;
        godotArraY[(int)RenderingServer.ArrayType.TexUV] = uvs;
        int indeX = 0;
        int[] indice = new int[36];

        for (int i = 0; i < 36; i += 6)
        {
            indice[i] = indeX;
            indice[i + 1] = indeX + 1;
            indice[i + 2] = indeX + 2;
            indice[i + 3] = indeX + 2;
            indice[i + 4] = indeX + 3;
            indice[i + 5] = indeX;

            indeX += 4;
        }

        godotArraY[(int)RenderingServer.ArrayType.Index] = indice;

        rawChunk.arrays[0] = godotArraY;
        rawChunk.materials[0] = material;
        rawChunk.colliderFaces[0] = vertice;

        return rawChunk;
    }
}
