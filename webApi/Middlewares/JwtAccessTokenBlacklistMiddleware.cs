using System.Security.Authentication;
using Microsoft.AspNetCore.Authorization;
using IAuthorizationService = Service.Abstract.Auth.IAuthorizationService;

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
        var endpointIsNotAnonymous = context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() == null;
        var accessToken = context.Request.Headers.Authorization.ToString()?.Replace("Bearer", string.Empty)?.Trim();

        if (endpointIsNotAnonymous && await _authorizationService.IsTokenBlacklisted(accessToken))
        {
            throw new AuthenticationException("Access token is blacklisted!");
        }

        await next(context);
    }
}
