using System.Buffers;
using System.Collections.Generic;
using System.Linq;
public class MeshingUtils {

    public static Position[] NaiveGreedyMeshing (Chunk chunk, ArrayPool<Position> pool) {
        Position pos = new Position ();
        Position pos1 = new Position ();
        Position pos2 = new Position ();
        Position pos3 = new Position ();

        int materials = chunk.Materials - 1;
        Position[] vertices = pool.Rent (Constants.CHUNK_SIZE2D * materials * 6 * 4);
        int currentLocation = 0;
        int staticOffset = materials * 6 * 4;
        int count = 0;
        int prev = 0;
        int objectID = 0;
        int lenght;
        int lastID = 0;

        for (int i = 0; count < Constants.CHUNK_SIZE3D; i++) {

            if (prev > 0) {
                i--;
                lenght = prev;
                prev = 0;
            } else {
                Run run = chunk.Voxels[i];
                objectID = run.value;
                lenght = run.lenght;
            }

            if (objectID == 0) {
                count += lenght;
                continue;
            }

            int z = count / Constants.CHUNK_SIZE2D;
            int y = count % Constants.CHUNK_SIZE1D;
            int x = (count / Constants.CHUNK_SIZE1D) % Constants.CHUNK_SIZE1D;

            if (lenght / Constants.CHUNK_SIZE1D > 0) {
                int size = Constants.CHUNK_SIZE1D - y;
                prev = lenght - size;
                lenght = size;
            }

            if (lastID == objectID) {
                lastID = 0;
            }

            int ax = x + 1;
            int ay = lenght + y;
            int az = z + 1;

            int location = (x + (z * Constants.CHUNK_SIZE1D)) * materials * 6;

            if (ay != y) {

                //Front
                int offset;

                currentLocation = location * 4;

                //1
                pos.x = x;
                pos.y = y;
                pos.z = z;
                pos.id = objectID;

                //2
                pos1.x = ax;
                pos1.y = y;
                pos1.z = z;
                pos1.id = objectID;

                //3
                pos2.x = ax;
                pos2.y = ay;
                pos2.z = z;
                pos2.id = objectID;

                //4
                pos3.x = x;
                pos3.y = ay;
                pos3.z = z;
                pos3.id = objectID;

                if (z > 0) {
                    for (int index = 0; index < materials; index++) {
                        offset = (Constants.CHUNK_SIZE1D * staticOffset) - (index * 4);
                        if (vertices[currentLocation - offset].id != 0) {
                            if (vertices[2 + currentLocation - offset].y >= ay) {
                                pos.id = -1;
                            } else if (vertices[2 + currentLocation - offset].y < ay && vertices[currentLocation - offset].y >= y) {
                                pos.y = vertices[2 + currentLocation - offset].y;
                                pos1.y = vertices[2 + currentLocation - offset].y;
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
                pos.x = ax;
                pos.y = y;
                pos.z = az;
                pos.id = objectID;

                //2
                pos1.x = x;
                pos1.y = y;
                pos1.z = az;
                pos1.id = objectID;

                //3
                pos2.x = x;
                pos2.y = ay;
                pos2.z = az;
                pos2.id = objectID;

                //4
                pos3.x = ax;
                pos3.y = ay;
                pos3.z = az;
                pos3.id = objectID;

                if (z > 0) {
                    for (int index = 0; index < materials; index++) {
                        offset = Constants.CHUNK_SIZE1D * staticOffset - (index * 4);
                        if (vertices[currentLocation - offset].id != 0) {
                            if (vertices[2 + currentLocation - offset].y > ay && vertices[currentLocation - offset].y <= y) {
                                vertices[currentLocation - offset].y = ay;
                                vertices[1 + currentLocation - offset].y = ay;
                            } else if (vertices[2 + currentLocation - offset].y <= ay) {
                                for (int s = 0; s < 4; s++) {
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
                pos.x = ax;
                pos.y = y;
                pos.z = z;
                pos.id = objectID;

                //2
                pos1.x = ax;
                pos1.y = y;
                pos1.z = az;
                pos1.id = objectID;

                //3
                pos2.x = ax;
                pos2.y = ay;
                pos2.z = az;
                pos2.id = objectID;

                //4
                pos3.x = ax;
                pos3.y = ay;
                pos3.z = z;
                pos3.id = objectID;

                if (x > 0) {
                    for (int index = 0; index < materials; index++) {
                        offset = staticOffset - (index * 4);
                        if (vertices[currentLocation - offset].id != 0) {
                            if (vertices[currentLocation - offset + 2].y > ay && vertices[currentLocation - offset].y <= y) {
                                vertices[currentLocation - offset].y = ay;
                                vertices[currentLocation - offset + 1].y = ay;
                            } else if (vertices[currentLocation - offset + 2].y <= ay) {
                                for (int s = 0; s < 4; s++) {
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
                pos.x = x;
                pos.y = y;
                pos.z = az;
                pos.id = objectID;

                //2
                pos1.x = x;
                pos1.y = y;
                pos1.z = z;
                pos1.id = objectID;

                //3
                pos2.x = x;
                pos2.y = ay;
                pos2.z = z;
                pos2.id = objectID;

                //4
                pos3.x = x;
                pos3.y = ay;
                pos3.z = az;
                pos3.id = objectID;

                if (x > 0) {
                    for (int index = 0; index < materials; index++) {
                        offset = staticOffset - (index * 4);
                        if (vertices[currentLocation - offset].id != 0) {
                            if (vertices[currentLocation - offset + 2].y >= ay) {
                                pos.id = -1;
                            } else if (vertices[currentLocation - offset + 2].y < ay && vertices[currentLocation - offset].y >= y) {
                                pos.y = vertices[currentLocation - offset + 2].y;
                                pos1.y = vertices[currentLocation - offset + 2].y;
                            }
                        }
                    }
                }

                vertices[currentLocation + (lastID * 4)] = pos;
                vertices[currentLocation + 1 + (lastID * 4)] = pos1;
                vertices[currentLocation + 2 + (lastID * 4)] = pos2;
                vertices[currentLocation + 3 + (lastID * 4)] = pos3;

                //Top
                int sx = x;
                offset = staticOffset;

                currentLocation = (lastID + 4 * materials + location) * 4;

                if (y > 0 && lastID > 0) {
                    if (vertices[((lastID - 1) + 4 * materials + location) * 4].id != 0) {
                        for (int s = 0; s < 4; s++) {
                            vertices[s + (((lastID - 1) + 4 * materials + location) * 4)].id = 0;
                        }
                    }
                }

                //Naive Greedy Meshing
                if (x > 0) {
                    if (vertices[currentLocation - offset].id != 0 && vertices[currentLocation - offset].y == ay) {
                        sx = vertices[currentLocation - offset].x;
                        for (int s = 0; s < 4; s++) {
                            vertices[s + currentLocation - offset].id = 0;
                        }
                    }
                }

                //1
                pos.x = sx;
                pos.y = ay;
                pos.z = z;
                pos.id = objectID;

                //2
                pos1.x = ax;
                pos1.y = ay;
                pos1.z = z;
                pos1.id = objectID;

                //3
                pos2.x = ax;
                pos2.y = ay;
                pos2.z = az;
                pos2.id = objectID;

                //4
                pos3.x = sx;
                pos3.y = ay;
                pos3.z = az;
                pos3.id = objectID;

                vertices[currentLocation] = pos;
                vertices[currentLocation + 1] = pos1;
                vertices[currentLocation + 2] = pos2;
                vertices[currentLocation + 3] = pos3;

                //Bottom
                offset = staticOffset;

                currentLocation = (5 * materials + location) * 4;

                if (y > 0 && lastID > 0) {
                    if (vertices[((lastID - 1) + 5 * materials + location) * 4].id != 0) {
                        count += lenght;
                        lastID = objectID;
                        if (lastID == materials) {
                            lastID = 0;
                        }
                        continue;
                    }
                }

                //Naive Greedy Meshing
                sx = x;
                if (x > 0) {
                    for (int index = 0; index < materials; index++) {
                        offset = staticOffset - (index * 4);
                        if (vertices[currentLocation - offset].id != 0 && vertices[currentLocation - offset].y == y) {
                            sx = vertices[1 + currentLocation - offset].x;
                            for (int s = 0; s < 4; s++) {
                                vertices[s + currentLocation - offset].id = 0;
                            }
                        }
                    }
                }

                //1
                pos.x = ax;
                pos.y = y;
                pos.z = z;
                pos.id = objectID;

                //2
                pos1.x = sx;
                pos1.y = y;
                pos1.z = z;
                pos1.id = objectID;

                //3
                pos2.x = sx;
                pos2.y = y;
                pos2.z = az;
                pos2.id = objectID;

                //4
                pos3.x = ax;
                pos3.y = y;
                pos3.z = az;
                pos3.id = objectID;

                vertices[currentLocation + (lastID * 4)] = pos;
                vertices[currentLocation + 1 + (lastID * 4)] = pos1;
                vertices[currentLocation + 2 + (lastID * 4)] = pos2;
                vertices[currentLocation + 3 + (lastID * 4)] = pos3;
            }

            lastID = objectID;

            if (lastID == materials) {
                lastID = 0;
            }

            count += lenght;
        }

        return vertices;
    }

    public static Queue<Position>[] GreedyMeshing (Position[] vertices, int side, Queue<Position>[] stack) {

        int count = stack.Count ();
        int staticOffset = 4 * count * 6;
        int offset = Constants.CHUNK_SIZE1D * staticOffset;

        for (int z = 0; z < Constants.CHUNK_SIZE1D; z++) {
            for (int x = 0; x < Constants.CHUNK_SIZE1D; x++) {
                for (int y = 0; y < count; y++) {
                    int location = (y + ((side + ((x + (z * Constants.CHUNK_SIZE1D)) * 6)) * count)) * 4;

                    Position pos = vertices[location];
                    Position pos1 = vertices[location + 1];
                    Position pos2 = vertices[location + 2];
                    Position pos3 = vertices[location + 3];

                    if (pos.id > 0) {
                        switch (side) {
                            case 0:
                                if (x < Constants.CHUNK_SIZE1D - 1 &&
                                    vertices[location + staticOffset].id > 0 &&
                                    vertices[location + staticOffset].y == pos.y &&
                                    pos2.y == vertices[location + staticOffset + 2].y) {

                                    pos1.x = vertices[location + staticOffset + 1].x;
                                    pos2.x = vertices[location + staticOffset + 2].x;

                                    vertices[location + staticOffset] = pos;
                                    vertices[location + staticOffset + 1] = pos1;
                                    vertices[location + staticOffset + 2] = pos2;
                                    vertices[location + staticOffset + 3] = pos3;

                                    for (int i = 0; i < 4; i++) {
                                        vertices[location + i].id = 0;
                                    }
                                    continue;
                                }
                                for (int i = 0; i < 4; i++) {
                                    Position insert = vertices[location + i];
                                    stack[insert.id - 1].Enqueue (insert);
                                }
                                break;
                            case 1:
                                if (x < Constants.CHUNK_SIZE1D - 1 &&
                                    vertices[location + staticOffset].id > 0 &&
                                    vertices[location + staticOffset + 2].y == pos2.y &&
                                    pos.y == vertices[location + staticOffset].y) {

                                    pos.x = vertices[location + staticOffset].x;
                                    pos3.x = vertices[location + staticOffset + 3].x;

                                    vertices[location + staticOffset] = pos;
                                    vertices[location + staticOffset + 1] = pos1;
                                    vertices[location + staticOffset + 2] = pos2;
                                    vertices[location + staticOffset + 3] = pos3;

                                    for (int i = 0; i < 4; i++) {
                                        vertices[location + i].id = 0;
                                    }
                                    continue;
                                }

                                for (int i = 0; i < 4; i++) {
                                    Position insert = vertices[location + i];
                                    stack[insert.id - 1].Enqueue (insert);
                                }

                                break;
                            case 2:
                                if (z < Constants.CHUNK_SIZE1D - 1 &&
                                    vertices[location + offset].id > 0 &&
                                    pos2.y == vertices[location + offset + 2].y &&
                                    pos.y == vertices[location + offset].y) {

                                    pos1.z = vertices[location + offset + 1].z;
                                    pos2.z = vertices[location + offset + 2].z;

                                    vertices[location + offset] = pos;
                                    vertices[location + offset + 1] = pos1;
                                    vertices[location + offset + 2] = pos2;
                                    vertices[location + offset + 3] = pos3;

                                    for (int i = 0; i < 4; i++) {
                                        vertices[location + i].id = 0;
                                    }
                                    continue;
                                }

                                for (int i = 0; i < 4; i++) {
                                    Position insert = vertices[location + i];
                                    stack[insert.id - 1].Enqueue (insert);
                                }
                                break;
                            case 3:
                                offset = Constants.CHUNK_SIZE1D * staticOffset;
                                if (z < Constants.CHUNK_SIZE1D - 1 &&
                                    vertices[location + offset].id > 0 &&
                                    vertices[location + offset].y == pos.y &&
                                    vertices[location + offset + 2].y == pos2.y) {

                                    pos.z = vertices[location + offset].z;
                                    pos3.z = vertices[location + offset + 3].z;

                                    vertices[location + offset] = pos;
                                    vertices[location + offset + 1] = pos1;
                                    vertices[location + offset + 2] = pos2;
                                    vertices[location + offset + 3] = pos3;

                                    for (int i = 0; i < 4; i++) {
                                        vertices[location + i].id = 0;
                                    }
                                    continue;
                                }
                                for (int i = 0; i < 4; i++) {
                                    Position insert = vertices[location + i];
                                    stack[insert.id - 1].Enqueue (insert);
                                }
                                break;
                            case 4:
                                if (z < Constants.CHUNK_SIZE1D - 1 &&
                                    vertices[location + offset].id > 0 &&
                                    vertices[location + offset].x == pos.x &&
                                    vertices[location + offset + 1].x == pos1.x &&
                                    vertices[location + offset].y == pos.y) {

                                    pos3.z = vertices[location + offset + 3].z;
                                    pos2.z = vertices[location + offset + 2].z;

                                    vertices[location + offset] = pos;
                                    vertices[location + offset + 1] = pos1;
                                    vertices[location + offset + 2] = pos2;
                                    vertices[location + offset + 3] = pos3;

                                    for (int i = 0; i < 4; i++) {
                                        vertices[location + i].id = 0;
                                    }
                                    continue;
                                }

                                for (int i = 0; i < 4; i++) {
                                    Position insert = vertices[location + i];
                                    stack[insert.id - 1].Enqueue (insert);
                                }
                                break;

                            case 5:
                                if (z < Constants.CHUNK_SIZE1D - 1 &&
                                    vertices[location + offset].id > 0 &&
                                    vertices[location + offset].x == pos.x &&
                                    vertices[location + offset + 2].x == pos2.x &&
                                    vertices[location + offset].y == pos.y) {

                                    pos3.z = vertices[location + offset + 3].z;
                                    pos2.z = vertices[location + offset + 2].z;

                                    vertices[location + offset] = pos;
                                    vertices[location + offset + 1] = pos1;
                                    vertices[location + offset + 2] = pos2;
                                    vertices[location + offset + 3] = pos3;

                                    for (int i = 0; i < 4; i++) {
                                        vertices[location + i].id = 0;
                                    }
                                    continue;
                                }

                                for (int i = 0; i < 4; i++) {
                                    Position insert = vertices[location + i];
                                    stack[insert.id - 1].Enqueue (insert);
                                }
                                break;
                        }
                    }
                    for (int i = 0; i < 4; i++) {
                        vertices[location + i].id = 0;
                    }
                }
            }
        }
        return stack;
    }
}