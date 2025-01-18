    namespace Emblix.Models;
    public class MovieGenre
    {
        public int MovieId { get; set; }
        public int GenreId { get; set; }
        
        // Навигационные свойства
        public Movie Movie { get; set; }
        public Genre Genre { get; set; }
    }