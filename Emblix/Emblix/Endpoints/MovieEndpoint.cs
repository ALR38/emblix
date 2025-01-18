using Emblix.Core.Templates;
using Emblix.Models;
using EmblixLibrary.Attributes;
using EmblixLibrary.Core;
using EmblixLibrary.Response;
using EmblixLibrary.Session;
using Microsoft.Data.SqlClient;
using ORM;
using System.Web; // Добавлено для декодирования

public class MovieEndpoint : BaseEndpoint
{
    private readonly string _connectionString = @"Server=localhost,1433;Database=EmblixDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;";

    [Get("movie")]
public IResponseResult GetMovie()
{
    try
    {
        string movieIdStr = Context.Request.QueryString["id"];
        if (!int.TryParse(movieIdStr, out int id))
        {
            return Redirect("/");
        }

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var movieContext = new ORMContext<Movie>(connection);
        var movie = movieContext.ReadById(id);

        if (movie == null)
        {
            return Redirect("/");
        }

        // Декодируем описание
        movie.Description = HttpUtility.UrlDecode(movie.Description);

        // Загружаем жанры
        var movieGenreContext = new ORMContext<MovieGenre>(connection);
        var genreContext = new ORMContext<Genre>(connection);
        var movieGenres = movieGenreContext.ReadByAll($"MovieId = {id}")
            .Select(mg => genreContext.ReadById(mg.GenreId))
            .Where(g => g != null)
            .Select(g => new { g.Name, g.Slug })
            .ToList();

        // Загружаем актеров
        var movieActorContext = new ORMContext<MovieActor>(connection);
        var actorContext = new ORMContext<Actor>(connection);
        var movieActors = movieActorContext.ReadByAll($"MovieId = {id}")
            .Select(ma => new
            {
                Name = HttpUtility.UrlDecode(actorContext.ReadById(ma.ActorId)?.Name),
                PhotoUrl = actorContext.ReadById(ma.ActorId)?.PhotoUrl,
                Role = HttpUtility.UrlDecode(ma.Role)
            })
            .Where(ma => ma.Name != null && ma.PhotoUrl != null)
            .ToList();

        // Добавляем загрузку отзывов
        var reviewContext = new ORMContext<Review>(connection);
        var reviews = reviewContext.ReadByAll($"MovieId = {id} AND isApproved = 1")
            .Select(r => new
            {
                AuthorName = HttpUtility.UrlDecode(r.AuthorName),
                Content = HttpUtility.UrlDecode(r.Content),
                Rating = r.Rating
            })
            .ToList();

        // Модель для шаблона (добавляем только Reviews)
        var model = new
        {
            movie.Id,
            movie.Title,
            movie.OriginalTitle,
            movie.Slug,
            movie.Description,
            movie.ReleaseYear,
            movie.Country,
            movie.Duration,
            movie.PosterUrl,
            movie.BackgroundUrl,
            movie.KinopoiskUrl,
            movie.ImdbUrl,
            movie.PlayerUrls,
            movie.Views,
            movie.Likes,
            MovieGenres = movieGenres,
            MovieActors = movieActors,
            Reviews = reviews,  // Добавили только это
            IsAuthorized = IsAuthorized(Context)
        };

        var template = GetTemplate("Pages/Movie/movie.html");
        var templator = new Templator();
        return Html(templator.GetHtmlByTemplate(template, model));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка в GetMovie: {ex.Message}");
        return Redirect("/");
    }
}


    
    [Post("api/reviews")]
public IResponseResult AddReview()
{
    try
    {
        // Читаем данные из тела запроса так же, как в AddMovie
        var formData = ParseFormData(Context.RequestBody);
        if (formData == null || !formData.Any())
        {
            return Json(new { success = false, message = "Данные формы отсутствуют" });
        }

        // Проверяем и преобразуем данные
        if (!int.TryParse(formData["movieId"], out int movieId) || 
            !int.TryParse(formData["rating"], out int rating) ||
            string.IsNullOrEmpty(formData["content"]) ||
            string.IsNullOrEmpty(formData["authorName"]))
        {
            return Redirect("/movie?id=" + formData["movieId"]);
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var reviewQuery = @"INSERT INTO reviews (movieId, rating, content, authorName, isApproved) 
                        VALUES (@movieId, @rating, @content, @authorName, @isApproved)";

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = reviewQuery;
                        
                        command.Parameters.AddWithValue("@movieId", movieId);
                        command.Parameters.AddWithValue("@rating", rating);
                        command.Parameters.AddWithValue("@content", HttpUtility.UrlEncode(formData["content"]));
                        command.Parameters.AddWithValue("@authorName", HttpUtility.UrlEncode(formData["authorName"]));
                        command.Parameters.AddWithValue("@isApproved", false);

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return Redirect($"/movie?id={movieId}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Ошибка при добавлении отзыва: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    return Redirect($"/movie?id={movieId}");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Общая ошибка: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        return Redirect("/");
    }
}

}