using System.Text;
using Common.Dto.Auth;
using Common.Exceptions;
using Common.Models.Passkey;
using DAL.Repositories.Abstract;
using Domain.Abstract;
using Service.Abstract.Auth;
using Service.Abstract.Auth.Passkeys;

namespace Service.Auth.Passkeys;

public class PasskeyAuthService(
    IPasskeyCryptographyService passkeyCryptographyService,
    IPasskeyRepository passkeyRepository,
    IPasskeySessionsService passkeySessionsService,
    IAuthorizationService authorizationService,
    IUnitOfWork unitOfWork) : IPasskeyAuthService
{
    public async Task<PasskeyRegistrationOptionsModel> GetOrCreateRegistrationSession(int userId, CancellationToken ct)
    {
        var user = await passkeyRepository.GetUserWithActivePasskeys(userId, ct)
            ?? throw new NotFoundException("User was not found!");

        var registrationSession = await passkeySessionsService.GetOngoingRegistrationSession(user.Id, ct)
            ?? await passkeySessionsService.CreateRegistrationSession(user, ct);

        return new PasskeyRegistrationOptionsModel
        {
            Challenge = Convert.ToBase64String(registrationSession.Challenge),
            RelyingParty = new PasskeyRelyingPartyModel
            {
                Id = registrationSession.Rp.Id,
                Name = registrationSession.Rp.Name,
                Icon = registrationSession.Rp.Icon
            },
            User = new PasskeyUserEntityModel
            {
                Id = Encoding.UTF8.GetString(registrationSession.User.Id),
                DisplayName = registrationSession.User.DisplayName,
                Name = registrationSession.User.Name
            },
            ExcludeCredentials = user.Passkeys.Select(p => new PasskeyCredentialModel(p.CredentialId))
        };
    }

    public async Task Register(RegisterPasskeyRequest request, int userId, CancellationToken ct)
    {
        var user = await passkeyRepository.GetUserWithActivePasskeys(userId, ct)
            ?? throw new NotFoundException($"User with ID: {userId} not found");

        var ongoingRegistrationSession = await passkeySessionsService.GetOngoingRegistrationSession(userId, ct)
            ?? throw new NotFoundException($"Registration session not found for ID:{userId}!");

        var passkey = await passkeyCryptographyService.ValidateRegistration(request, user, ongoingRegistrationSession, ct);

        await passkeyRepository.AddAsync(passkey, ct);
        await unitOfWork.CommitAsync(ct);
    }

    public async Task<PasskeyAuthenticationOptionsModel> StartAuthenticationSession(CancellationToken ct)
    {
        var session = await passkeySessionsService.CreateAuthenticationSession(ct);
        return session;
    }

    public async Task<AuthorizationResponse> Authenticate(AuthenticatePasskeyRequest request, CancellationToken ct)
    {
        var authenticationSession = await passkeySessionsService.GetOngoingAuthenticationSession(request.Challenge, ct)
                                    ?? throw new NotFoundException("Authentication session not found!");
        await passkeySessionsService.RemoveOngoingAuthenticationSession(request.Challenge, ct);
        var userId = request.UserId;
        if (!userId.HasValue || userId < 0)
        {
            throw new ValidationException("Could not parse User Id");
        }

        var user = await passkeyRepository.GetUserWithActivePasskeys(userId.Value, ct)
            ?? throw new NotFoundException($"User with ID: {userId} was not found");

        if (user.IsBanned)
        {
            throw new ValidationException($"User is banned! Please contact administrator!");
        }

        if (!user.Passkeys.Any())
        {
            throw new NotFoundException("No passkeys available!");
        }

        await passkeyCryptographyService.ValidateAuthentication(request, user, authenticationSession, ct);
        var authResponse = await authorizationService.Authorize(user, ct);
        return authResponse;
    }
}