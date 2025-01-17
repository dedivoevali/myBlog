﻿using API.Controllers.Base;
using API.Extensions;
using AutoMapper;
using Common.Dto.Auth;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Service.Abstract.Auth.Passkeys;

namespace API.Controllers.Auth;

[FeatureGate("Passkey")]
[Route("api/passkey")]
public class PasskeyAuthController(IPasskeyAuthService passkeyAuthService, IMapper mapper) : AppBaseController
{

    [HttpGet("registration-options")]
    public async Task<IActionResult> GetRegistrationOptions(CancellationToken ct)
    {
        var registrationOptions = await passkeyAuthService.GetOrCreateRegistrationSession(CurrentUserId, ct);
        return Ok(registrationOptions);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterPasskeyRequest request, CancellationToken ct)
    {
        await passkeyAuthService.Register(request, CurrentUserId, ct);
        return Created();
    }

    [AllowAnonymous]
    [HttpGet("authentication-options")]
    public async Task<IActionResult> GetAuthenticationOptions(CancellationToken ct)
    {
        var authenticationOptions = await passkeyAuthService.StartAuthenticationSession(ct);
        return Ok(authenticationOptions);
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate(AuthenticatePasskeyRequest request, CancellationToken ct)
    {
        var authenticateResponse = await passkeyAuthService.Authenticate(request, ct);
        HttpContext.AddRefreshTokenCookie(authenticateResponse.RefreshToken, authenticateResponse.RefreshTokenExpiresAt);
        return Ok(mapper.Map<AuthorizationResponseModel>(authenticateResponse));
    }

    [HttpDelete("{id:int:min(0)}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await passkeyAuthService.Deactivate(id, CurrentUserId, ct);
        return Ok();
    }
}