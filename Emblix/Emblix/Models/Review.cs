namespace Emblix.Models;
public class Review
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public int? Rating { get; set; }
    public string Content { get; set; }
    public string AuthorName { get; set; }
    public bool isApproved { get; set; }

    public Movie Movie { get; set; }
}