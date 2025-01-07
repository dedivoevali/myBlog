using System.Security.Authentication;
using Service.Abstract.Auth;

namespace API.Middlewares;

public class JwtAccessTokenBlacklistMiddleware : IMiddleware
{
    private readonly IAuthorizationService _authorizationService;

    public JwtAccessTokenBlacklistMiddleware(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var accessToken = context.Request.Headers.Authorization.ToString()?.Replace("Bearer", string.Empty)?.Trim();
        if (await _authorizationService.IsTokenBlacklisted(accessToken))
        {
            throw new AuthenticationException("Access token is blacklisted!");
        }

        await next(context);
    }
}
