namespace Common.Dto.Auth;

public record struct RefreshToken(string Value, DateTime ExpiresAt);