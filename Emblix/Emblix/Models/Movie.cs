using Emblix.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string OriginalTitle { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public int? ReleaseYear { get; set; }
    public string Country { get; set; }
    public int? Duration { get; set; }
    public string PosterUrl { get; set; }
    public string BackgroundUrl { get; set; }
    public string KinopoiskUrl { get; set; }
    public string ImdbUrl { get; set; }
    public string PlayerUrls { get; set; }
    public int Views { get; set; }
    public int Likes { get; set; }
    public decimal Rating { get; set; }

    // Навигационные свойства
    public ICollection<MovieGenre> MovieGenres { get; set; }
    public ICollection<MovieActor> MovieActors { get; set; }
    public ICollection<UserFavorite> Favorites { get; set; }
    public ICollection<Review> Reviews { get; set; }

    // Вычисляемые свойства для шаблонизатора
    public string Year => ReleaseYear?.ToString() ?? "";
    public List<string> GenreNames => MovieGenres?.Select(mg => mg.Genre?.Name).Where(n => n != null).ToList() ?? new List<string>();
    public List<string> ActorNames => MovieActors?.Select(ma => ma.Actor?.Name).Where(n => n != null).ToList() ?? new List<string>();
    public string GenresString => string.Join(", ", GenreNames);
    public double? AverageRating => Reviews?.Where(r => r.Rating.HasValue).Select(r => r.Rating.Value).DefaultIfEmpty().Average();
}