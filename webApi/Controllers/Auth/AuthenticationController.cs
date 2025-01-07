using API.Controllers.Base;
using AutoMapper;
using Common.Dto.Auth;
using Common.Models;
using Common.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Abstract.Auth;

namespace API.Controllers.Auth
{
    [Route("api/auth")]
    public class AuthenticationController : AppBaseController
    {
        private readonly IPasswordAuthService _passwordAuthService;
        private readonly Service.Abstract.Auth.IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;

        public AuthenticationController(
            IPasswordAuthService passwordAuthService,
            Service.Abstract.Auth.IAuthorizationService authorizationService,
            IMapper mapper)
        {
            _passwordAuthService = passwordAuthService;
            _authorizationService = authorizationService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] PasswordAuthorizeRequest userData,
            CancellationToken cancellationToken)
        {
            var authenticationResponse = await _passwordAuthService.AuthenticateAsync(userData, cancellationToken);

            HttpContext.Response.Cookies.Append(
                JwtUtils.CookieRefreshTokenKey,
                authenticationResponse.RefreshToken,
                new CookieOptions { HttpOnly = true, Secure = true, Expires = authenticationResponse.RefreshTokenExpiresAt, SameSite = SameSiteMode.None });
            
            return Ok(_mapper.Map<AuthorizationResponseModel>(authenticationResponse));
        }

        [AllowAnonymous]
        [HttpGet("refresh-access-token")]
        public async Task<IActionResult> RefreshAccessToken(CancellationToken ct)
        {
            var refreshToken = HttpContext.Request.Cookies[JwtUtils.CookieRefreshTokenKey];

            var newToken = await _authorizationService.GetNewAccessToken(refreshToken, ct);
            return Ok(new AuthorizationResponseModel { AccessToken = newToken });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var accessToken = HttpContext.Request.Headers.Authorization.ToString()
                ?.Replace(JwtBearerDefaults.AuthenticationScheme, string.Empty).Trim();

            await _authorizationService.PurgeRefreshToken(CurrentUserId, ct);
            await _authorizationService.BlacklistAccessToken(accessToken, ct);
            HttpContext.Response.Cookies.Delete(JwtUtils.CookieRefreshTokenKey);
            return Ok();
        }
    }
}