namespace EmblixLibrary.Session;

public static class SessionStorage
{
    private static readonly Dictionary<string, int> _sessions = new Dictionary<string, int>();
 
    // Сохранение токена и его соответствующего ID пользователя
    public static void SaveSession(string token, int userId)
    {
        _sessions[token] = userId;
    }
 
    // Проверка токена
    public static bool ValidateToken(string token)
    {
        return _sessions.ContainsKey(token);
    }
 
    // Получение ID пользователя по токену
    public static int? GetUserId(string token)
    {
        return _sessions.TryGetValue(token, out var userId) ? userId : null;
    }
    
    private static readonly Dictionary<string, bool> _adminStatus = new Dictionary<string, bool>();

    public static void SaveSession(string token, int userId, bool isAdmin)
    {
        _sessions[token] = userId;
        _adminStatus[token] = isAdmin;
    }

    public static bool IsAdmin(string token)
    {
        return _adminStatus.ContainsKey(token) && _adminStatus[token];
    }
}