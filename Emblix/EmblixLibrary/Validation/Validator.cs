using System.Text.RegularExpressions;

namespace EmblixLibrary.Validation;

public static class Validator
{
    // Минимум 8 символов, минимум 1 буква и 1 цифра
    private static readonly Regex PasswordRegex = new(
        @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$",
        RegexOptions.Compiled
    );

    // Проверка email
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled
    );

    public static (bool isValid, string error) ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return (false, "Пароль не может быть пустым");

        if (!PasswordRegex.IsMatch(password))
            return (false, "Пароль должен содержать минимум 8 символов, включая буквы и цифры");

        return (true, string.Empty);
    }

    public static (bool isValid, string error) ValidateEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return (false, "Email не может быть пустым");

        if (!EmailRegex.IsMatch(email))
            return (false, "Неверный формат email");

        return (true, string.Empty);
    }
}