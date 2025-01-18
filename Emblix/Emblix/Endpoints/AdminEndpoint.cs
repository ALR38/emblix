using System.Collections.Specialized;
using System.Data;
using System.Net;
using Emblix.Core.Templates;
using Emblix.Models;
using Emblix.Session;
using EmblixLibrary.Attributes;
using EmblixLibrary.Core;
using EmblixLibrary.Response;
using EmblixLibrary.Session;
using Microsoft.Data.SqlClient;
using ORM;

public class AdminEndpoint : BaseEndpoint
{
    private readonly ORMContext<Review> _reviewContext;
    private readonly ORMContext<Movie> _movieContext;
    private readonly ORMContext<Genre> _genreContext;
    private readonly ORMContext<Actor> _actorContext;
    private readonly ORMContext<MovieGenre> _movieGenreContext;
    private readonly ORMContext<MovieActor> _movieActorContext;
    private readonly ORMContext<User> _userContext;
    private readonly IDbConnection _connection;
    private readonly string _connectionString = @"Server=localhost,1433;Database=EmblixDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;";

    public AdminEndpoint()
    {
        _connection = new SqlConnection(_connectionString);
        
        _reviewContext = new ORMContext<Review>(_connection);
        _movieContext = new ORMContext<Movie>(_connection);
        _genreContext = new ORMContext<Genre>(_connection);
        _actorContext = new ORMContext<Actor>(_connection);
        _movieGenreContext = new ORMContext<MovieGenre>(_connection);
        _movieActorContext = new ORMContext<MovieActor>(_connection);
        _userContext = new ORMContext<User>(_connection);
    }
    
    public AdminEndpoint(IDbConnection connection)
    {
        _connection = connection;
        _reviewContext = new ORMContext<Review>(connection);
        _movieContext = new ORMContext<Movie>(connection);
        _genreContext = new ORMContext<Genre>(connection);
        _actorContext = new ORMContext<Actor>(connection);
        _movieGenreContext = new ORMContext<MovieGenre>(connection);
        _movieActorContext = new ORMContext<MovieActor>(connection);
        _userContext = new ORMContext<User>(connection);
    }

    [Get("admin")]
    public IResponseResult AdminPanel()
    {
        if (!IsAdmin())
        {
            return Redirect("emblix");
        }

        try
        {
            var template = File.ReadAllText("Templates/Pages/Admin/admin.html");
            var genres = _genreContext.ReadByAll();
            var movies = _movieContext.ReadByAll();
            
            var moviesWithGenres = movies.Select(movie =>
            {
                var movieGenres = _movieGenreContext
                    .ReadByAll($"movieId = {movie.Id}")
                    .Select(mg => _genreContext.ReadById(mg.GenreId))
                    .Where(g => g != null)
                    .Select(g => new { g.Name })
                    .ToList();

                return new
                {
                    movie.Id,
                    movie.Title,
                    movie.OriginalTitle,
                    movie.PosterUrl,
                    Genres = movieGenres
                };
            }).ToList();
            
            var model = new
            {
                Genres = genres,
                Movies = moviesWithGenres
            };
            
            var templator = new Templator();
            return Html(templator.GetHtmlByTemplate(template, model));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в AdminPanel: {ex.Message}");
            return Html("<h1>Произошла ошибка при загрузке админ-панели</h1>");
        }
    }

    [Post("admin/movie/add")]
    public IResponseResult AddMovie()
    {
        if (!IsAdmin())
        {
            return Json(new { success = false, message = "Доступ запрещен" });
        }

        var formData = ParseFormData(Context.RequestBody);
        if (formData == null || !formData.Any())
        {
            return Json(new { success = false, message = "Данные формы отсутствуют" });
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var movie = new Movie
                    {
                        Title = PrepareText(formData["title"]),
                        OriginalTitle = PrepareText(formData["originalTitle"]),
                        Slug = GenerateSlug(formData["title"]),
                        Description = PrepareText(formData["description"]),
                        ReleaseYear = int.TryParse(formData["releaseYear"], out var year) ? year : null,
                        Country = PrepareText(formData["country"]),
                        Duration = int.TryParse(formData["duration"], out var duration) ? duration : null,
                        PosterUrl = formData["posterUrl"],
                        BackgroundUrl = formData["backgroundUrl"],
                        KinopoiskUrl = formData["kinopoiskUrl"],
                        ImdbUrl = formData["imdbUrl"],
                        PlayerUrls = formData["playerUrls"],
                        Views = 0,
                        Likes = 0
                    };

                    var movieQuery = @"INSERT INTO movies (title, originalTitle, slug, description, releaseYear, country, 
                        duration, posterUrl, backgroundUrl, kinopoiskUrl, imdbUrl, playerUrls, views, likes) 
                        VALUES (@title, @originalTitle, @slug, @description, @releaseYear, @country, @duration, 
                        @posterUrl, @backgroundUrl, @kinopoiskUrl, @imdbUrl, @playerUrls, @views, @likes); 
                        SELECT SCOPE_IDENTITY();";

                    int movieId;
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = movieQuery;
                        
                        command.Parameters.AddWithValue("@title", movie.Title);
                        command.Parameters.AddWithValue("@originalTitle", (object)movie.OriginalTitle ?? DBNull.Value);
                        command.Parameters.AddWithValue("@slug", movie.Slug);
                        command.Parameters.AddWithValue("@description", (object)movie.Description ?? DBNull.Value);
                        command.Parameters.AddWithValue("@releaseYear", (object)movie.ReleaseYear ?? DBNull.Value);
                        command.Parameters.AddWithValue("@country", (object)movie.Country ?? DBNull.Value);
                        command.Parameters.AddWithValue("@duration", (object)movie.Duration ?? DBNull.Value);
                        command.Parameters.AddWithValue("@posterUrl", movie.PosterUrl);
                        command.Parameters.AddWithValue("@backgroundUrl", movie.BackgroundUrl);
                        command.Parameters.AddWithValue("@kinopoiskUrl", (object)movie.KinopoiskUrl ?? DBNull.Value);
                        command.Parameters.AddWithValue("@imdbUrl", (object)movie.ImdbUrl ?? DBNull.Value);
                        command.Parameters.AddWithValue("@playerUrls", (object)movie.PlayerUrls ?? DBNull.Value);
                        command.Parameters.AddWithValue("@views", movie.Views);
                        command.Parameters.AddWithValue("@likes", movie.Likes);

                        movieId = Convert.ToInt32(command.ExecuteScalar());
                    }

                    if (!string.IsNullOrEmpty(formData["genres"]))
                    {
                        var genreIds = formData["genres"].Split(',').Select(int.Parse);
                        foreach (var genreId in genreIds)
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = "INSERT INTO movieGenres (movieId, genreId) VALUES (@movieId, @genreId)";
                                command.Parameters.AddWithValue("@movieId", movieId);
                                command.Parameters.AddWithValue("@genreId", genreId);
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(formData["actors"]) && formData["actors"] != "[]")
                    {
                        var actors = System.Text.Json.JsonSerializer.Deserialize<List<dynamic>>(formData["actors"]);
                        foreach (var actorData in actors)
                        {
                            int actorId;
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = @"INSERT INTO actors (name, photoUrl) 
                                    VALUES (@name, @photoUrl); SELECT SCOPE_IDENTITY();";
                                command.Parameters.AddWithValue("@name", actorData.GetProperty("name").GetString());
                                command.Parameters.AddWithValue("@photoUrl", actorData.GetProperty("photoUrl").GetString());
                                actorId = Convert.ToInt32(command.ExecuteScalar());
                            }

                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = "INSERT INTO movieActors (movieId, actorId, role) VALUES (@movieId, @actorId, @role)";
                                command.Parameters.AddWithValue("@movieId", movieId);
                                command.Parameters.AddWithValue("@actorId", actorId);
                                command.Parameters.AddWithValue("@role", actorData.GetProperty("role").GetString());
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();
                    return Json(new { success = true, message = "Фильм успешно добавлен" });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Ошибка при добавлении фильма: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    return Json(new { success = false, message = $"Ошибка: {ex.Message}" });
                }
            }
        }
    }

    [Post("admin/movie/delete")]
    public IResponseResult DeleteMovie()
    {
        if (!IsAdmin())
        {
            return Redirect("emblix");
        }

        try
        {
            var formData = ParseFormData(Context.RequestBody);
            if (!formData.ContainsKey("movieId") || !int.TryParse(formData["movieId"], out int movieId))
            {
                return Json(new { success = false, message = "Некорректный ID фильма" });
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var deleteGenresCmd = new SqlCommand(
                            "DELETE FROM movieGenres WHERE movieId = @movieId", 
                            connection, 
                            transaction);
                        deleteGenresCmd.Parameters.AddWithValue("@movieId", movieId);
                        deleteGenresCmd.ExecuteNonQuery();

                        var deleteActorsCmd = new SqlCommand(
                            "DELETE FROM movieActors WHERE movieId = @movieId", 
                            connection, 
                            transaction);
                        deleteActorsCmd.Parameters.AddWithValue("@movieId", movieId);
                        deleteActorsCmd.ExecuteNonQuery();

                        var deleteMovieCmd = new SqlCommand(
                            "DELETE FROM movies WHERE id = @movieId", 
                            connection, 
                            transaction);
                        deleteMovieCmd.Parameters.AddWithValue("@movieId", movieId);
                        var rowsAffected = deleteMovieCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            transaction.Commit();
                            return Redirect("../..");
                        }
                        else
                        {
                            transaction.Rollback();
                            return Json(new { success = false, message = "Фильм не найден" });
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Ошибка при удалении фильма: {ex.Message}");
                        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                        return Json(new { success = false, message = $"Ошибка при удалении: {ex.Message}" });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Общая ошибка: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            return Json(new { success = false, message = $"Произошла ошибка: {ex.Message}" });
        }
    }

    private bool IsAdmin()
    {
        Cookie token = Context.Request.Cookies["session-token"];
        if (token == null || !SessionStorage.ValidateToken(token.Value))
        {
            return false;
        }

        var userId = SessionStorage.GetUserId(token.Value);
        if (!userId.HasValue)
        {
            return false;
        }

        var user = _userContext.ReadById(userId.Value);
        return user != null && user.IsAdmin;
    }

    private string PrepareText(string text) => text?.Trim() ?? string.Empty;

    private string GenerateSlug(string title)
    {
        if (string.IsNullOrEmpty(title)) return string.Empty;

        var slug = Transliteration.CyrillicToLatin(title);
        return string.Join("-", slug.Split(new[] { ' ', '.', ',', '!', '?', ':', ';', '"', '\'', '(', ')' }, 
            StringSplitOptions.RemoveEmptyEntries)).ToLower();
    }
}