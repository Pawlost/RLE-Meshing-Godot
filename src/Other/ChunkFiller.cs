public class ChunkFiller {
    private volatile int dirtID;
    private volatile int grassID;
    public ChunkFiller (int dirtID, int grassID) {
        this.dirtID = dirtID;
        this.grassID = grassID;
    }
    public Chunk GenerateChunk (float posX, float posY, float posZ, Weltschmerz weltschmerz) {
        Run run = new Run ();
        Chunk chunk = new Chunk ();
        chunk.Borders = new int[384];

        chunk.x = (uint) posX;
        chunk.y = (uint) posY;
        chunk.z = (uint) posZ;

        chunk.Materials = 1;

        Run[] voxels = new Run[Constants.CHUNK_SIZE3D];

        chunk.IsEmpty = true;

        int posx = (int) (posX * 4);
        int posz = (int) (posZ * 4);
        int posy = (int) (posY * 4);

        int lastPosition = 0;

        chunk.IsSurface = false;
        for (int z = 0; z < Constants.CHUNK_SIZE1D; z++) {
            for (int x = 0; x < Constants.CHUNK_SIZE1D; x++) {
                int elevation = (int) weltschmerz.GetElevation (x + posx, z + posz);

                if (elevation / Constants.CHUNK_SIZE1D == posy / Constants.CHUNK_SIZE1D) {

                    if (elevation > 0) {
                        int position = elevation % Constants.CHUNK_SIZE1D;

                        if (lastPosition > 0) {
                            run = voxels[lastPosition - 1];
                            if (run.value == dirtID) {
                                run.lenght = position + run.lenght;
                                voxels[lastPosition - 1] = run;
                            } else {
                                run.lenght = position;
                                run.value = dirtID;
                                voxels[lastPosition] = run;
                                lastPosition++;
                            }
                        } else {
                            run.lenght = position;
                            run.value = dirtID;
                            voxels[lastPosition] = run;
                            lastPosition++;
                        }

                        run.lenght = 1;
                        run.value = grassID;
                        voxels[lastPosition] = run;
                        lastPosition++;

                        run.lenght = Constants.CHUNK_SIZE1D - (elevation % Constants.CHUNK_SIZE1D) - 1;
                        run.value = 0;
                        voxels[lastPosition] = run;
                        lastPosition++;

                        chunk.IsSurface = true;
                        chunk.IsEmpty = false;
                    } else {
                        int position = Constants.CHUNK_SIZE1D;

                        if (lastPosition > 0) {
                            run = voxels[lastPosition - 1];
                            if (run.value == 0) {
                                run.lenght = position + run.lenght;
                                voxels[lastPosition - 1] = run;
                                continue;
                            }
                        }

                        run.lenght = position;
                        run.value = 0;
                        voxels[lastPosition] = run;
                        lastPosition++;
                    }
                } else if (elevation / Constants.CHUNK_SIZE1D > posy / Constants.CHUNK_SIZE1D) {

                    int position = Constants.CHUNK_SIZE1D;
                    if (lastPosition > 0) {
                        run = voxels[lastPosition - 1];
                        if (run.value == dirtID) {
                            run.lenght = position + run.lenght;
                            voxels[lastPosition - 1] = run;
                            continue;
                        }
                    }

                    run.lenght = position;
                    run.value = dirtID;

                    voxels[lastPosition] = run;
                    lastPosition++;

                    chunk.IsEmpty = false;

                } else if (elevation / Constants.CHUNK_SIZE1D < posy / Constants.CHUNK_SIZE1D) {

                    int position = Constants.CHUNK_SIZE1D;

                    if (lastPosition > 0) {
                        run = voxels[lastPosition - 1];
                        if (run.value == 0) {
                            run.lenght = position + run.lenght;
                            voxels[lastPosition - 1] = run;
                            continue;
                        }
                    }

                    run.lenght = position;
                    run.value = 0;

                    voxels[lastPosition] = run;
                    lastPosition++;
                }
            }
        }

        if (chunk.IsSurface) {
            chunk.Materials = 3;
        }

        chunk.Voxels = voxels;

        return chunk;
    }

}