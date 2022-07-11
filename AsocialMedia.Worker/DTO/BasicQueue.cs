namespace AsocialMedia.Worker.DTO;

public class BasicQueue
{
    public Destination Destination { get; set; } = new();
    public Asset Asset { get; set; } = new();
}
