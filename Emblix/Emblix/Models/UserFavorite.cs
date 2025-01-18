namespace Emblix.Models;

public class UserFavorite
{
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int MovieId { get; set; }
    public Movie Movie { get; set; }
    
    public bool IsFavorite { get; set; }
    public bool IsSubscribed { get; set; }
}