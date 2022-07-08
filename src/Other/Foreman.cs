using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
public class Foreman {
	//This is recommend max static octree size because it takes 134 MB
	private volatile Octree octree;
	//private int grassMeshID;
	private volatile Weltschmerz weltschmerz;
	private volatile Terra terra;
	private ChunkFiller chunkFiller;
	private ConcurrentQueue<Tuple<int, int, int>> queue;
	private volatile bool runThread = true;
	private volatile List<long> chunkSpeed;
	public volatile static int chunksLoaded = 0;
	public volatile static int chunksPlaced = 0;
	public volatile static int positionsScreened = 0;
	public volatile static int positionsInRange = 0;

	public volatile static int positionsNeeded = 0;

	private Stopwatch stopwatch;
	private ITerraSemaphore preparation;
	private ITerraSemaphore generation;

	private int Length = 0;

	private int maxSize;

	private int preparationThreads;

	public Foreman (Weltschmerz weltschmerz, Registry registry, Terra terra, int radius) {
		chunkFiller = new ChunkFiller (registry.SelectByName ("dirt").worldID, registry.SelectByName ("grass").worldID);

		this.weltschmerz = weltschmerz;
		this.terra = terra;
		for(int x = 0; x < radius; x ++){
			for(int y = 0; y < radius; y ++){
				for(int z = 0; z < radius; z ++){
					if (terra.CheckBoundries (x, y, z)) {
						OctreeNode node = terra.TraverseOctree(x, y, z, 0);
						if(node != null){
							//Godot.GD.Print("here");
							LoadArea (x << Constants.CHUNK_EXPONENT, y << Constants.CHUNK_EXPONENT, z << Constants.CHUNK_EXPONENT, node);
						}
					}
				}
			}
		}
	}

	//Loads chunks
	private void LoadArea (int posX, int posY, int posZ, OctreeNode node) {

		Chunk chunk;
		if (posY > weltschmerz.GetConfig ().elevation.max_elevation) {
			chunk = new Chunk ();
			chunk.IsEmpty = true;
		} else {
			chunk = chunkFiller.GenerateChunk (posX, posY, posZ, weltschmerz);
		}

		chunk.x = (uint) posX;
		chunk.y = (uint) posY;
		chunk.z = (uint) posZ;

		node.chunk = chunk;
	}

	public void Stop () {
		runThread = false;
	}

	public List<long> GetMeasures () {
		return chunkSpeed;
	}
}