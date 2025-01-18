using System.Net;
using Emblix.Core.Templates;
using Emblix.Models;
using Microsoft.Data.SqlClient; 
using EmblixLibrary.Attributes;
using EmblixLibrary.Core;
using EmblixLibrary.Response;
using EmblixLibrary.Session;
using EmblixLibrary.Validation;
using ORM;
using TemplateEngine;

namespace Emblix.Endpoints;

public class AuthEndpoint : BaseEndpoint
{
    private readonly string _connectionString = @"Server=localhost,1433;Database=EmblixDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;";
    
    [Get("auth")]
    public IResponseResult GetAuthentication()
    {
        var template = File.ReadAllText(@"Templates/Pages/Auth/auth.html");
        var error = Context.Request.QueryString["error"] ?? "";
        
        var model = new { error = error };
        var templator = new Templator();
        return Html(templator.GetHtmlByTemplate(template, model));
    }

    [Post("check-email")]
    public IResponseResult CheckEmail()
    {
        // Получаем email из тела запроса
        var formData = ParseFormData(Context.RequestBody);
        var email = formData["email"];

        var (isValid, error) = Validator.ValidateEmail(email);
        if (!isValid)
            return Redirect($"auth?error={Uri.EscapeDataString(error)}");

        using var connection = new SqlConnection(_connectionString);
        var context = new ORMContext<User>(connection);

        var user = context.ReadByAll($"Email = '{email}'").FirstOrDefault();

        if (user != null)
        {
            return Redirect($"login?email={Uri.EscapeDataString(email)}");
        }
        else
        {
            return Redirect($"registration?email={Uri.EscapeDataString(email)}");
        }
    }
}

