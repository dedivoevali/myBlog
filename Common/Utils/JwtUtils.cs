namespace Common.Utils;

public static class JwtUtils
{
    public const string CookieRefreshTokenKey = "myblog-refresh-token";
    public static string GetJwtCacheKey(string token) => $"jwt-{token}";
}