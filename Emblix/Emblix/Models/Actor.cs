namespace Emblix.Models;

public class Actor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string PhotoUrl { get; set; }

    public ICollection<MovieActor> MovieActors { get; set; }
}