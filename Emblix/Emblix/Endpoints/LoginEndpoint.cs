using System.Net;
using Emblix.Core.Templates;
using Emblix.Models;
using EmblixLibrary.Attributes;
using EmblixLibrary.Core;
using EmblixLibrary.Response;
using EmblixLibrary.Session;
using EmblixLibrary.Validation;
using Microsoft.Data.SqlClient;
using ORM;

public class LoginEndpoint : BaseEndpoint
{
    private readonly string _connectionString = @"Server=localhost,1433;Database=EmblixDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;";
    
    [Get("login")]
    public IResponseResult GetLogin()
    {
        string email = Context.Request.QueryString["email"] ?? "";
        
        if (string.IsNullOrEmpty(email))
        {
            return Redirect("auth");
        }

        string errorMessage = Context.Request.QueryString["error"] ?? "";
        var template = File.ReadAllText("Templates/Pages/Login/login.html");
        var model = new { email = email, error = errorMessage };
        var templator = new Templator();
        
        return Html(templator.GetHtmlByTemplate(template, model));
    }

    [Post("login")]
    public IResponseResult PostLogin()
    {
        string email = string.Empty; // Объявляем переменную на уровне метода
    
        try
        {
            var formData = ParseFormData(Context.RequestBody);
            if (!formData.TryGetValue("email", out email) || !formData.TryGetValue("password", out var password))
            {
                ClearAuthData();
                return Redirect($"auth?error={Uri.EscapeDataString("Отсутствуют необходимые данные")}");
            }

            using var connection = new SqlConnection(_connectionString);
            var context = new ORMContext<User>(connection);

            var user = context.ReadByAll($"Email = '{email}'").FirstOrDefault();

            if (user == null || user.Password != password)
            {
                ClearAuthData();
                return Redirect($"auth?email={Uri.EscapeDataString(email)}&error={Uri.EscapeDataString("Неверный email или пароль")}");
            }

            // Если все ок, создаем сессию
            string token = Guid.NewGuid().ToString();
            var cookie = new Cookie("session-token", token)
            {
                Expires = DateTime.Now.AddDays(1)
            };
            Context.Response.SetCookie(cookie);
        
            SessionStorage.SaveSession(token, user.Id, user.IsAdmin);

            return user.IsAdmin ? Redirect("admin") : Redirect("emblix");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            ClearAuthData();
            return Redirect($"auth?error={Uri.EscapeDataString("Произошла ошибка при входе")}");
        }
    }

    private void ClearAuthData()
    {
        // Удаляем куки
        var cookie = new Cookie("session-token", "")
        {
            Expires = DateTime.Now.AddDays(-1) // Устанавливаем дату истечения в прошлом
        };
        Context.Response.SetCookie(cookie);

        // Очищаем сессию если есть
        var existingToken = Context.Request.Cookies["session-token"]?.Value;
        if (!string.IsNullOrEmpty(existingToken))
        {
            var userId = SessionStorage.GetUserId(existingToken);
            if (userId.HasValue)
            {
                SessionStorage.SaveSession(existingToken, userId.Value, false); // Сбрасываем права админа
            }
        }
    }
}