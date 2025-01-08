using System.Security.Authentication;
using Common.Dto.Auth;
using DAL.Repositories.Abstract;
using Domain;
using Common.Exceptions;
using Common.Options;
using Common.Utils;
using Common.Validation;
using Domain.Abstract;
using Microsoft.Extensions.Options;
using Service.Abstract;
using Service.Abstract.Auth;

namespace Service.Auth;

public class AuthorizationService : IAuthorizationService
{
    private readonly IEncryptionService _encryptionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;
    private readonly JsonWebTokenOptions _jsonWebTokenOptions;

    public AuthorizationService(IUnitOfWork unitOfWork,
        IEncryptionService encryptionService,
        IUserRepository userRepository,
        ICacheService cacheService,
        IOptions<JsonWebTokenOptions> options)
    {
        _jsonWebTokenOptions = options.Value;
        _unitOfWork = unitOfWork;
        _encryptionService = encryptionService;
        _userRepository = userRepository;
        _cacheService = cacheService;
    }

    public async Task<AuthorizationResponse> Authorize(User user, CancellationToken ct = default)
    {
        if (user.IsBanned)
        {
            throw new ValidationException("User is banned! Please contact platform administrators!");
        }

        var accessToken = _encryptionService.GenerateAccessToken(user.Id, user.Username);
        var refreshToken = _encryptionService.GenerateRefreshToken();

        user.RefreshToken = refreshToken.Value;
        user.LastActivity = DateTime.UtcNow;
        user.RefreshTokenExpiresAt = refreshToken.ExpiresAt;
        await _unitOfWork.CommitAsync(ct);

        return new AuthorizationResponse
        {
            UserId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Value,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt
        };
    }

    public async Task<string> GetNewAccessToken(string refreshToken, int userId, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ValidationException("Refresh token was not found");
            }
            if (refreshToken.Length != EntityConfigurationConstants.RefreshTokenLength)
            {
                throw new ValidationException($"Refresh token should be {EntityConfigurationConstants.RefreshTokenLength} characters long");
            }
            var user = await _userRepository.GetByIdAsync(userId, ct)
                       ?? throw new NotFoundException("User not found");

            if (string.Compare(user.RefreshToken, refreshToken, StringComparison.Ordinal) != 0)
            {
                throw new ValidationException($"Refresh token is invalid");
            }

            user.LastActivity = DateTime.UtcNow;
            await _unitOfWork.CommitAsync(ct);
            var accessToken = _encryptionService.GenerateAccessToken(user.Id, user.Username);
            return accessToken;
        }
        catch(Exception ex)
        {
            throw new AccessDeniedException(ex.Message);
        }
    }

    public async Task PurgeRefreshToken(int userId, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User not found");
        user.RefreshTokenExpiresAt = null;
        user.RefreshToken = null;
        await _unitOfWork.CommitAsync(ct);
    }

    public async Task BlacklistAccessToken(string? accessToken, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new ValidationException("Please provide current access token to log out");
        }

        var key = JwtUtils.GetJwtCacheKey(accessToken);
        var expiresAt = TimeSpan.FromMinutes(_jsonWebTokenOptions.AccessTokenValidityTimeMinutes);
        await _cacheService.SetAsync(key, "blacklisted", expiresAt, ct);
    }

    public async Task<bool> IsTokenBlacklisted(string? accessToken, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            return false;
        }

        var key = JwtUtils.GetJwtCacheKey(accessToken);
        return !string.IsNullOrEmpty(await _cacheService.GetStringAsync(key, ct));
    }
}