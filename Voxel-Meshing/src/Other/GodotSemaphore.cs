using Godot;

public partial class GodotSemaphore : ITerraSemaphore
{
    private Semaphore semaphore;
    public GodotSemaphore()
    {
        semaphore = new Semaphore();
    }
    public void Post()
    {
        semaphore.Post();
    }

    public void Wait()
    {
        semaphore.Wait();
    }
}


