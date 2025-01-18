using System.Data;
using Emblix.Core.Templates;
using Emblix.Models;
using EmblixLibrary.Attributes;
using EmblixLibrary.Core;
using EmblixLibrary.Response;
using Microsoft.Data.SqlClient;
using ORM;

public class EmblixEndpoint : BaseEndpoint
{
    private readonly ORMContext<Movie> _movieContext;
    private readonly ORMContext<Genre> _genreContext;
    private readonly ORMContext<MovieGenre> _movieGenreContext;
    private readonly IDbConnection _connection;
    private readonly string _connectionString = @"Server=localhost,1433;Database=EmblixDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;";

    private static readonly Dictionary<string, (string Slug, string Name)> GenreMapping = new()
    {
        { "biografii", ("biografii", "Биографии") },
        { "boeviki", ("boeviki", "Боевики") },
        { "vesterny", ("vesterny", "Вестерны") },
        { "voennye", ("voennye", "Военные") },
        { "detektivnye", ("detektivnye", "Детективные") },
        { "dokumentalnye", ("dokumentalnye", "Документальные") },
        { "dramy", ("dramy", "Драмы") },
        { "istoricheskie", ("istoricheskie", "Исторические") },
        { "komedii", ("komedii", "Комедии") },
        { "korotkometrazhnye", ("korotkometrazhnye", "Короткометражные") },
        { "melodramy", ("melodramy", "Мелодрамы") },
        { "muzykalnye", ("muzykalnye", "Музыкальные") },
        { "multfilmy", ("multfilmy", "Мультфильмы") },
        { "priklyucheniya", ("priklyucheniya", "Приключения") },
        { "semejnye", ("semejnye", "Семейные") },
        { "sport", ("sport", "Спорт") },
        { "tv", ("tv", "ТВ") },
        { "trillery", ("trillery", "Триллеры") },
        { "uzhasy", ("uzhasy", "Ужасы") },
        { "fantastika", ("fantastika", "Фантастика") },
        { "fentezi", ("fentezi", "Фэнтези") },
        { "dc", ("dc", "DC") },
        { "marvel", ("marvel", "Marvel") },
        { "netflix", ("netflix", "Netflix") }
    };

    public EmblixEndpoint()
    {
        _connection = new SqlConnection(_connectionString);
        _movieContext = new ORMContext<Movie>(_connection);
        _genreContext = new ORMContext<Genre>(_connection);
        _movieGenreContext = new ORMContext<MovieGenre>(_connection);
    }

 
    [Get("emblix")]
public IResponseResult GetMovies()
{
    try
    {
        var template = File.ReadAllText("Templates/Pages/Emblix/emblix.html");
        var selectedGenre = Context.Request.QueryString["genre"];
        var yearSort = Context.Request.QueryString["yearSort"];
        var page = int.TryParse(Context.Request.QueryString["page"], out int p) ? p : 1;
        var pageSize = 20;

        // Загружаем данные
        var movies = _movieContext.ReadByAll().ToList();
        var genres = _genreContext.ReadByAll().ToList();
        var movieGenres = _movieGenreContext.ReadByAll().ToList();

        // Привязываем жанры к фильмам
        foreach (var movie in movies)
        {
            movie.MovieGenres = movieGenres
                .Where(mg => mg.MovieId == movie.Id)
                .Select(mg =>
                {
                    mg.Genre = genres.FirstOrDefault(g => g.Id == mg.GenreId);
                    return mg;
                })
                .Where(mg => mg.Genre != null)
                .ToList();
        }

        // Фильтрация по жанру
        if (!string.IsNullOrEmpty(selectedGenre))
        {
            var genre = genres.FirstOrDefault(g => g.Slug == selectedGenre);
            if (genre != null)
            {
                movies = movies.Where(m => 
                    m.MovieGenres.Any(mg => mg.GenreId == genre.Id)).ToList();
            }
        }

        // Сортировка
        movies = !string.IsNullOrEmpty(yearSort) && yearSort == "desc" 
            ? movies.OrderByDescending(m => m.ReleaseYear).ToList()
            : movies.OrderBy(m => m.ReleaseYear).ToList(); 

        // Пагинация
        var totalMovies = movies.Count;
        var totalPages = (int)Math.Ceiling(totalMovies / (double)pageSize);
        movies = movies.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        // Формируем модель для шаблона
        var model = new
        {
            // Все жанры в одном списке
            Genres = GenreMapping
                .Select(g => new
                {
                    Slug = g.Value.Slug,
                    Name = g.Value.Name,
                    IsActive = g.Key == selectedGenre
                }).ToList(),

            // Фильмы
            Movies = movies.Select(m => new
            {
                Id = m.Id,
                Title = m.Title,
                ReleaseYear = m.ReleaseYear,
                Country = m.Country,
                PosterUrl = m.PosterUrl,
                Rating = m.Rating,
                Views = m.Views,
                Genres = m.MovieGenres
                    .Where(mg => mg.Genre != null)
                    .Select(mg => new 
                    { 
                        Name = mg.Genre.Name,
                        Slug = mg.Genre.Slug 
                    })
                    .ToList()
            }).ToList(),

            // Параметры страницы
            SelectedGenre = selectedGenre,
            YearSort = yearSort,
            CurrentPage = page,
            TotalPages = totalPages,
            HasPreviousPage = page > 1,
            HasNextPage = page < totalPages
        };

        var templator = new Templator();
        var result = templator.GetHtmlByTemplate(template, model);
        
        return Html(result);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in GetMovies: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        return Html($"<h1>Произошла ошибка: {ex.Message}</h1>");
    }
}
}
