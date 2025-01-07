using Common.Dto.Auth;
using Domain;

namespace Service.Abstract.Auth;

public interface IAuthorizationService
{
    Task<AuthorizationResponse> Authorize(User user, CancellationToken ct = default);
    Task<string> GetNewAccessToken(string refreshToken, CancellationToken ct = default);
    Task PurgeRefreshToken(int userId, CancellationToken ct = default);
    Task BlacklistAccessToken(string? accessToken, CancellationToken ct = default);
    Task<bool> IsTokenBlacklisted(string accessToken, CancellationToken ct = default);
}