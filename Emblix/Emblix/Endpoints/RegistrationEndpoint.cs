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
using TemplateEngine;

namespace Emblix.Endpoints;

public class RegistrationEndpoint : BaseEndpoint
{
    private readonly string _connectionString = @"Server=localhost,1433;Database=EmblixDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;";
    
    [Get("registration")]
    public IResponseResult GetRegistration()
    {
        string email = Context.Request.QueryString["email"] ?? "";
        
        if (string.IsNullOrEmpty(email))
        {
            return Redirect("auth");
        }

        string error = Context.Request.QueryString["error"] ?? "";
        var template = File.ReadAllText(@"Templates/Pages/Registration/registration.html");
        var model = new { email = email, error = error };
        var templator = new Templator();
        return Html(templator.GetHtmlByTemplate(template, model));
    }

    [Post("register")]
    public IResponseResult Register()
    {
        var formData = ParseFormData(Context.RequestBody);
        var email = formData["email"];
        var password = formData["password"];
        var confirmPassword = formData["confirmPassword"];

        var (isEmailValid, emailError) = Validator.ValidateEmail(email);
        if (!isEmailValid)
            return Redirect($"registration?email={Uri.EscapeDataString(email)}&error={Uri.EscapeDataString(emailError)}");

        var (isPasswordValid, passwordError) = Validator.ValidatePassword(password);
        if (!isPasswordValid)
            return Redirect($"registration?email={Uri.EscapeDataString(email)}&error={Uri.EscapeDataString(passwordError)}");

        if (password != confirmPassword)
            return Redirect($"registration?email={Uri.EscapeDataString(email)}&error=Пароли не совпадают");

        var connection = new SqlConnection(_connectionString);
        var context = new ORMContext<User>(connection);

        var existingUser = context.ReadByAll($"Email = '{email}'").FirstOrDefault();
        if (existingUser != null)
            return Redirect($"login?email={Uri.EscapeDataString(email)}");

        var newUser = new User
        {
            Email = email,
            Password = password,
            IsAdmin = false 
        };

        context.Create(newUser);

        var sessionToken = Guid.NewGuid().ToString();
        var sessionCookie = new Cookie("session-token", sessionToken);
        Context.Response.SetCookie(sessionCookie);
        SessionStorage.SaveSession(sessionToken, newUser.Id);

        return Redirect("emblix");
    }
}