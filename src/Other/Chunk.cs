public class Chunk
{
    public uint x { get; set; }
    public uint y { get; set; }
    public uint z { get; set; }
    public int Materials { get; set; }
    public Run[] Voxels { get; set; }
    public bool IsEmpty { get; set; }
    public bool IsSurface { get; set; }

    public int[] Borders{get; set;}
}