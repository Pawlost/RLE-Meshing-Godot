using Godot;
using Godotmeshing.src.Other;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Constants = Godotmeshing.src.Other.Constants;
public partial class MeshingUtils
{

    public static Position[] NaiveGreedyMeshing(Chunk chunk, ArrayPool<Position> pool)
    {
        Position pos = new Position();
        Position pos1 = new Position();
        Position pos2 = new Position();
        Position pos3 = new Position();

        int materials = chunk.Materials - 1;
        Position[] vertices = pool.Rent(Constants.CHUNK_SIZE2D * materials * 6 * 4);
        int currentLocation = 0;
        int staticOffset = materials * 6 * 4;
        int count = 0;
        int prev = 0;
        int objectID = 0;
        int lenght;
        int lastID = 0;

        for (int i = 0; count < Constants.CHUNK_SIZE3D; i++)
        {

            if (prev > 0)
            {
                i--;
                lenght = prev;
                prev = 0;
            }
            else
            {
                Run run = chunk.Voxels[i];
                objectID = run.value;
                lenght = run.lenght;
            }

            if (objectID == 0)
            {
                count += lenght;
                continue;
            }

            int Z = count / Constants.CHUNK_SIZE2D;
            int Y = count % Constants.CHUNK_SIZE1D;
            int X = (count / Constants.CHUNK_SIZE1D) % Constants.CHUNK_SIZE1D;

            if (lenght / Constants.CHUNK_SIZE1D > 0)
            {
                int siZe = Constants.CHUNK_SIZE1D - Y;
                prev = lenght - siZe;
                lenght = siZe;
            }

            if (lastID == objectID)
            {
                lastID = 0;
            }

            int aX = X + 1;
            int aY = lenght + Y;
            int aZ = Z + 1;

            int location = (X + (Z * Constants.CHUNK_SIZE1D)) * materials * 6;

            if (aY != Y)
            {

                //Front
                int offset;

                currentLocation = location * 4;

                //1
                pos.X = X;
                pos.Y = Y;
                pos.Z = Z;
                pos.id = objectID;

                //2
                pos1.X = aX;
                pos1.Y = Y;
                pos1.Z = Z;
                pos1.id = objectID;

                //3
                pos2.X = aX;
                pos2.Y = aY;
                pos2.Z = Z;
                pos2.id = objectID;

                //4
                pos3.X = X;
                pos3.Y = aY;
                pos3.Z = Z;
                pos3.id = objectID;

                if (Z > 0)
                {
                    for (int indeX = 0; indeX < materials; indeX++)
                    {
                        offset = (Constants.CHUNK_SIZE1D * staticOffset) - (indeX * 4);
                        if (vertices[currentLocation - offset].id != 0)
                        {
                            if (vertices[2 + currentLocation - offset].Y >= aY)
                            {
                                pos.id = -1;
                            }
                            else if (vertices[2 + currentLocation - offset].Y < aY && vertices[currentLocation - offset].Y >= Y)
                            {
                                pos.Y = vertices[2 + currentLocation - offset].Y;
                                pos1.Y = vertices[2 + currentLocation - offset].Y;
                            }
                        }
                    }
                }

                vertices[currentLocation + (lastID * 4)] = pos;
                vertices[currentLocation + 1 + (lastID * 4)] = pos1;
                vertices[currentLocation + 2 + (lastID * 4)] = pos2;
                vertices[currentLocation + 3 + (lastID * 4)] = pos3;

                //Back
                currentLocation = (materials + location) * 4;

                //1
                pos.X = aX;
                pos.Y = Y;
                pos.Z = aZ;
                pos.id = objectID;

                //2
                pos1.X = X;
                pos1.Y = Y;
                pos1.Z = aZ;
                pos1.id = objectID;

                //3
                pos2.X = X;
                pos2.Y = aY;
                pos2.Z = aZ;
                pos2.id = objectID;

                //4
                pos3.X = aX;
                pos3.Y = aY;
                pos3.Z = aZ;
                pos3.id = objectID;

                if (Z > 0)
                {
                    for (int indeX = 0; indeX < materials; indeX++)
                    {
                        offset = Constants.CHUNK_SIZE1D * staticOffset - (indeX * 4);
                        if (vertices[currentLocation - offset].id != 0)
                        {
                            if (vertices[2 + currentLocation - offset].Y > aY && vertices[currentLocation - offset].Y <= Y)
                            {
                                vertices[currentLocation - offset].Y = aY;
                                vertices[1 + currentLocation - offset].Y = aY;
                            }
                            else if (vertices[2 + currentLocation - offset].Y <= aY)
                            {
                                for (int s = 0; s < 4; s++)
                                {
                                    vertices[s + currentLocation - offset].id = 0;
                                }
                            }
                        }
                    }
                }

                vertices[currentLocation + (lastID * 4)] = pos;
                vertices[currentLocation + 1 + (lastID * 4)] = pos1;
                vertices[currentLocation + 2 + (lastID * 4)] = pos2;
                vertices[currentLocation + 3 + (lastID * 4)] = pos3;

                //Right
                currentLocation = (2 * materials + location) * 4;

                //1
                pos.X = aX;
                pos.Y = Y;
                pos.Z = Z;
                pos.id = objectID;

                //2
                pos1.X = aX;
                pos1.Y = Y;
                pos1.Z = aZ;
                pos1.id = objectID;

                //3
                pos2.X = aX;
                pos2.Y = aY;
                pos2.Z = aZ;
                pos2.id = objectID;

                //4
                pos3.X = aX;
                pos3.Y = aY;
                pos3.Z = Z;
                pos3.id = objectID;

                if (X > 0)
                {
                    for (int indeX = 0; indeX < materials; indeX++)
                    {
                        offset = staticOffset - (indeX * 4);
                        if (vertices[currentLocation - offset].id != 0)
                        {
                            if (vertices[currentLocation - offset + 2].Y > aY && vertices[currentLocation - offset].Y <= Y)
                            {
                                vertices[currentLocation - offset].Y = aY;
                                vertices[currentLocation - offset + 1].Y = aY;
                            }
                            else if (vertices[currentLocation - offset + 2].Y <= aY)
                            {
                                for (int s = 0; s < 4; s++)
                                {
                                    vertices[s + currentLocation - offset].id = 0;
                                }
                            }
                        }
                    }
                }

                vertices[currentLocation + (lastID * 4)] = pos;
                vertices[currentLocation + 1 + (lastID * 4)] = pos1;
                vertices[currentLocation + 2 + (lastID * 4)] = pos2;
                vertices[currentLocation + 3 + (lastID * 4)] = pos3;

                //Left
                currentLocation = (3 * materials + location) * 4;

                //1
                pos.X = X;
                pos.Y = Y;
                pos.Z = aZ;
                pos.id = objectID;

                //2
                pos1.X = X;
                pos1.Y = Y;
                pos1.Z = Z;
                pos1.id = objectID;

                //3
                pos2.X = X;
                pos2.Y = aY;
                pos2.Z = Z;
                pos2.id = objectID;

                //4
                pos3.X = X;
                pos3.Y = aY;
                pos3.Z = aZ;
                pos3.id = objectID;

                if (X > 0)
                {
                    for (int indeX = 0; indeX < materials; indeX++)
                    {
                        offset = staticOffset - (indeX * 4);
                        if (vertices[currentLocation - offset].id != 0)
                        {
                            if (vertices[currentLocation - offset + 2].Y >= aY)
                            {
                                pos.id = -1;
                            }
                            else if (vertices[currentLocation - offset + 2].Y < aY && vertices[currentLocation - offset].Y >= Y)
                            {
                                pos.Y = vertices[currentLocation - offset + 2].Y;
                                pos1.Y = vertices[currentLocation - offset + 2].Y;
                            }
                        }
                    }
                }

                vertices[currentLocation + (lastID * 4)] = pos;
                vertices[currentLocation + 1 + (lastID * 4)] = pos1;
                vertices[currentLocation + 2 + (lastID * 4)] = pos2;
                vertices[currentLocation + 3 + (lastID * 4)] = pos3;

                //Top
                int sX = X;
                offset = staticOffset;

                currentLocation = (lastID + 4 * materials + location) * 4;

                if (Y > 0 && lastID > 0)
                {
                    if (vertices[((lastID - 1) + 4 * materials + location) * 4].id != 0)
                    {
                        for (int s = 0; s < 4; s++)
                        {
                            vertices[s + (((lastID - 1) + 4 * materials + location) * 4)].id = 0;
                        }
                    }
                }

                //Naive GreedY Meshing
                if (X > 0)
                {
                    if (vertices[currentLocation - offset].id != 0 && vertices[currentLocation - offset].Y == aY)
                    {
                        sX = vertices[currentLocation - offset].X;
                        for (int s = 0; s < 4; s++)
                        {
                            vertices[s + currentLocation - offset].id = 0;
                        }
                    }
                }

                //1
                pos.X = sX;
                pos.Y = aY;
                pos.Z = Z;
                pos.id = objectID;

                //2
                pos1.X = aX;
                pos1.Y = aY;
                pos1.Z = Z;
                pos1.id = objectID;

                //3
                pos2.X = aX;
                pos2.Y = aY;
                pos2.Z = aZ;
                pos2.id = objectID;

                //4
                pos3.X = sX;
                pos3.Y = aY;
                pos3.Z = aZ;
                pos3.id = objectID;

                vertices[currentLocation] = pos;
                vertices[currentLocation + 1] = pos1;
                vertices[currentLocation + 2] = pos2;
                vertices[currentLocation + 3] = pos3;

                //Bottom
                offset = staticOffset;

                currentLocation = (5 * materials + location) * 4;

                if (Y > 0 && lastID > 0)
                {
                    if (vertices[((lastID - 1) + 5 * materials + location) * 4].id != 0)
                    {
                        count += lenght;
                        lastID = objectID;
                        if (lastID == materials)
                        {
                            lastID = 0;
                        }
                        continue;
                    }
                }

                //Naive GreedY Meshing
                sX = X;
                if (X > 0)
                {
                    for (int indeX = 0; indeX < materials; indeX++)
                    {
                        offset = staticOffset - (indeX * 4);
                        if (vertices[currentLocation - offset].id != 0 && vertices[currentLocation - offset].Y == Y)
                        {
                            sX = vertices[1 + currentLocation - offset].X;
                            for (int s = 0; s < 4; s++)
                            {
                                vertices[s + currentLocation - offset].id = 0;
                            }
                        }
                    }
                }

                //1
                pos.X = aX;
                pos.Y = Y;
                pos.Z = Z;
                pos.id = objectID;

                //2
                pos1.X = sX;
                pos1.Y = Y;
                pos1.Z = Z;
                pos1.id = objectID;

                //3
                pos2.X = sX;
                pos2.Y = Y;
                pos2.Z = aZ;
                pos2.id = objectID;

                //4
                pos3.X = aX;
                pos3.Y = Y;
                pos3.Z = aZ;
                pos3.id = objectID;

                vertices[currentLocation + (lastID * 4)] = pos;
                vertices[currentLocation + 1 + (lastID * 4)] = pos1;
                vertices[currentLocation + 2 + (lastID * 4)] = pos2;
                vertices[currentLocation + 3 + (lastID * 4)] = pos3;
            }

            lastID = objectID;

            if (lastID == materials)
            {
                lastID = 0;
            }

            count += lenght;
        }

        return vertices;
    }

    public static Queue<Position>[] GreedyMeshing(Position[] vertices, int side, Queue<Position>[] stack)
    {

        int count = stack.Count();
        int staticOffset = 4 * count * 6;
        int offset = Constants.CHUNK_SIZE1D * staticOffset;

        for (int Z = 0; Z < Constants.CHUNK_SIZE1D; Z++)
        {
            for (int X = 0; X < Constants.CHUNK_SIZE1D; X++)
            {
                for (int Y = 0; Y < count; Y++)
                {
                    int location = (Y + ((side + ((X + (Z * Constants.CHUNK_SIZE1D)) * 6)) * count)) * 4;

                    Position pos = vertices[location];
                    Position pos1 = vertices[location + 1];
                    Position pos2 = vertices[location + 2];
                    Position pos3 = vertices[location + 3];

                    if (pos.id > 0)
                    {
                        switch (side)
                        {
                            case 0:
                                if (X < Constants.CHUNK_SIZE1D - 1 &&
                                    vertices[location + staticOffset].id > 0 &&
                                    vertices[location + staticOffset].Y == pos.Y &&
                                    pos2.Y == vertices[location + staticOffset + 2].Y)
                                {

                                    pos1.X = vertices[location + staticOffset + 1].X;
                                    pos2.X = vertices[location + staticOffset + 2].X;

                                    vertices[location + staticOffset] = pos;
                                    vertices[location + staticOffset + 1] = pos1;
                                    vertices[location + staticOffset + 2] = pos2;
                                    vertices[location + staticOffset + 3] = pos3;

                                    for (int i = 0; i < 4; i++)
                                    {
                                        vertices[location + i].id = 0;
                                    }
                                    continue;
                                }
                                for (int i = 0; i < 4; i++)
                                {
                                    Position insert = vertices[location + i];
                                    stack[insert.id - 1].Enqueue(insert);
                                }
                                break;
                            case 1:
                                if (X < Constants.CHUNK_SIZE1D - 1 &&
                                    vertices[location + staticOffset].id > 0 &&
                                    vertices[location + staticOffset + 2].Y == pos2.Y &&
                                    pos.Y == vertices[location + staticOffset].Y)
                                {

                                    pos.X = vertices[location + staticOffset].X;
                                    pos3.X = vertices[location + staticOffset + 3].X;

                                    vertices[location + staticOffset] = pos;
                                    vertices[location + staticOffset + 1] = pos1;
                                    vertices[location + staticOffset + 2] = pos2;
                                    vertices[location + staticOffset + 3] = pos3;

                                    for (int i = 0; i < 4; i++)
                                    {
                                        vertices[location + i].id = 0;
                                    }
                                    continue;
                                }

                                for (int i = 0; i < 4; i++)
                                {
                                    Position insert = vertices[location + i];
                                    stack[insert.id - 1].Enqueue(insert);
                                }

                                break;
                            case 2:
                                if (Z < Constants.CHUNK_SIZE1D - 1 &&
                                    vertices[location + offset].id > 0 &&
                                    pos2.Y == vertices[location + offset + 2].Y &&
                                    pos.Y == vertices[location + offset].Y)
                                {

                                    pos1.Z = vertices[location + offset + 1].Z;
                                    pos2.Z = vertices[location + offset + 2].Z;

                                    vertices[location + offset] = pos;
                                    vertices[location + offset + 1] = pos1;
                                    vertices[location + offset + 2] = pos2;
                                    vertices[location + offset + 3] = pos3;

                                    for (int i = 0; i < 4; i++)
                                    {
                                        vertices[location + i].id = 0;
                                    }
                                    continue;
                                }

                                for (int i = 0; i < 4; i++)
                                {
                                    Position insert = vertices[location + i];
                                    stack[insert.id - 1].Enqueue(insert);
                                }
                                break;
                            case 3:
                                offset = Constants.CHUNK_SIZE1D * staticOffset;
                                if (Z < Constants.CHUNK_SIZE1D - 1 &&
                                    vertices[location + offset].id > 0 &&
                                    vertices[location + offset].Y == pos.Y &&
                                    vertices[location + offset + 2].Y == pos2.Y)
                                {

                                    pos.Z = vertices[location + offset].Z;
                                    pos3.Z = vertices[location + offset + 3].Z;

                                    vertices[location + offset] = pos;
                                    vertices[location + offset + 1] = pos1;
                                    vertices[location + offset + 2] = pos2;
                                    vertices[location + offset + 3] = pos3;

                                    for (int i = 0; i < 4; i++)
                                    {
                                        vertices[location + i].id = 0;
                                    }
                                    continue;
                                }
                                for (int i = 0; i < 4; i++)
                                {
                                    Position insert = vertices[location + i];
                                    stack[insert.id - 1].Enqueue(insert);
                                }
                                break;
                            case 4:
                                if (Z < Constants.CHUNK_SIZE1D - 1 &&
                                    vertices[location + offset].id > 0 &&
                                    vertices[location + offset].X == pos.X &&
                                    vertices[location + offset + 1].X == pos1.X &&
                                    vertices[location + offset].Y == pos.Y)
                                {

                                    pos3.Z = vertices[location + offset + 3].Z;
                                    pos2.Z = vertices[location + offset + 2].Z;

                                    vertices[location + offset] = pos;
                                    vertices[location + offset + 1] = pos1;
                                    vertices[location + offset + 2] = pos2;
                                    vertices[location + offset + 3] = pos3;

                                    for (int i = 0; i < 4; i++)
                                    {
                                        vertices[location + i].id = 0;
                                    }
                                    continue;
                                }

                                for (int i = 0; i < 4; i++)
                                {
                                    Position insert = vertices[location + i];
                                    stack[insert.id - 1].Enqueue(insert);
                                }
                                break;

                            case 5:
                                if (Z < Constants.CHUNK_SIZE1D - 1 &&
                                    vertices[location + offset].id > 0 &&
                                    vertices[location + offset].X == pos.X &&
                                    vertices[location + offset + 2].X == pos2.X &&
                                    vertices[location + offset].Y == pos.Y)
                                {

                                    pos3.Z = vertices[location + offset + 3].Z;
                                    pos2.Z = vertices[location + offset + 2].Z;

                                    vertices[location + offset] = pos;
                                    vertices[location + offset + 1] = pos1;
                                    vertices[location + offset + 2] = pos2;
                                    vertices[location + offset + 3] = pos3;

                                    for (int i = 0; i < 4; i++)
                                    {
                                        vertices[location + i].id = 0;
                                    }
                                    continue;
                                }

                                for (int i = 0; i < 4; i++)
                                {
                                    Position insert = vertices[location + i];
                                    stack[insert.id - 1].Enqueue(insert);
                                }
                                break;
                        }
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        vertices[location + i].id = 0;
                    }
                }
            }
        }
        return stack;
    }
}


