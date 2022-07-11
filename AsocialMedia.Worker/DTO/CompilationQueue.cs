namespace AsocialMedia.Worker.DTO;

public class CompilationQueue
{
    public Destination Destination { get; set; } = new();
    public List<Asset> Assets { get; set; } = new List<Asset> { };
}
