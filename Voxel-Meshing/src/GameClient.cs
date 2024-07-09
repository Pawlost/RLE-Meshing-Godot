using System;
using System.Buffers;
using System.Diagnostics;
using System.Threading;
using Godot;
using Godot.NativeInterop;
using Godotmeshing.src.Other;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using static Godot.HttpClient;
using Constants = Godotmeshing.src.Other.Constants;
public partial class GameClient : Node3D
{

    [Export] public int CHUNKS_TO_GENERATE = 20;
    private ChunkFiller chunkFiller;

    private GodotMesher mesher;

    private Weltschmerz weltschmerz;

    public override void _Ready()
    {
        Registry reg = new Registry();
        PrimitiveResources.Register(reg);
        mesher = new GodotMesher();
        mesher.SetRegistry(reg);
        chunkFiller = new ChunkFiller(1, 2);
        weltschmerz = new Weltschmerz();

        Thread thread = new Thread(() =>
        {
            // Code to run on the new thread goes here.
            Generate();
        });

        thread.Start();

        registry = new Registry();
        PrimitiveResources.Register(registry);

        Config config = new Config();
        config.elevation.max_elevation = MAX_ELEVATION;
        config.elevation.min_elevation = MIN_ELEVATION;
        config.map.latitude = LATITUDE;
        config.map.longitude = LONGITUDE;

        if (LONGITUDE < 2)
        {
            LONGITUDE = 2;
        }

        if (LATITUDE < 2)
        {
            LATITUDE = 2;
        }

        if (MAX_ELEVATION < 2)
        {
            MAX_ELEVATION = 2;
        }

        Position boundries = new Position();
        boundries.X = LONGITUDE;
        boundries.Y = MAX_ELEVATION;
        boundries.Z = LATITUDE;

        terra = new Terra(boundries, this);
        GodotSemaphore semaphore1 = new GodotSemaphore();
        GodotSemaphore semaphore2 = new GodotSemaphore();

        foreman = new Foreman(weltschmerz, registry, terra, LOAD_RADIUS);

        mesher.SetRegistry(registry);
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionPressed("ui_cancel"))
        {
            GetTree().Quit();
        }
    }

    public void Generate()
    {
        ArrayPool<Position> pool = ArrayPool<Position>.Create(Constants.CHUNK_SIZE3D * 6 * 4, 1);
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        for (int x = 0; x < CHUNKS_TO_GENERATE; x++)
        {
            for (int y = 0; y < CHUNKS_TO_GENERATE; y++)
            {
                for (int z = 0; z < CHUNKS_TO_GENERATE; z++)
                {
                    Chunk chunk = chunkFiller.GenerateChunk(x << Constants.CHUNK_EXPONENT, y << Constants.CHUNK_EXPONENT,
                        z << Constants.CHUNK_EXPONENT, weltschmerz);
                    if (!chunk.IsSurface)
                    {
                        chunk.x = (uint)x << Constants.CHUNK_EXPONENT;
                        chunk.y = (uint)y << Constants.CHUNK_EXPONENT;
                        chunk.z = (uint)z << Constants.CHUNK_EXPONENT;
                    }

                    if (!chunk.IsEmpty)
                    {
                        var instancedChunk = mesher.MeshChunk(chunk, pool);

                        CallDeferred(nameof(InstanceChunk), instancedChunk);
                    }
                }
            }
        }

        stopwatch.Stop();
        Godot.GD.Print(CHUNKS_TO_GENERATE * CHUNKS_TO_GENERATE * CHUNKS_TO_GENERATE + " chunks generated in " + stopwatch.ElapsedMilliseconds);
    }


    private void InstanceChunk(MeshInstance3D mesh)
    {
        mesh.CreateTrimeshCollision();
     //   mesh.CreateConvexCollision();
        GetParent().GetParent().AddChild(mesh);
    }


    public override void _Process(double delta) { 
        
    }

    //Exports
    [Export] public int LOAD_RADIUS = 10;
    [Export] public int SEED = 19083;
    [Export] public int LONGITUDE = 1000;
    [Export] public int LATITUDE = 1000;
    [Export] public int MAX_ELEVATION = 1000;
    [Export] public int MIN_ELEVATION = 1;
    [Export] public int PROCESS_THREADS = 1;

    private Registry registry;

    private volatile Terra terra;

    private Foreman foreman;

    // Called when the node enters the scene tree for the first time.

    public Chunk RequestChunk(int x, int y, int z)
    {
        OctreeNode node = terra.TraverseOctree(x, y, z, 0);
        if (node != null)
        {
            return node.chunk;
        }
        return null;
    }

    public void Clear()
    {
        foreman.Stop();
    }
}