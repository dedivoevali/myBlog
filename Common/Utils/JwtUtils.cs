namespace Common.Utils;

public static class JwtUtils
{
    public const string CookieRefreshTokenKey = "refreshtoken";
    public static string GetJwtCacheKey(string token) => $"jwt-{token}";
}