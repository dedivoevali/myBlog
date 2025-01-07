using Common.Dto.Auth;

namespace Service.Abstract.Auth
{
    public interface IPasswordAuthService
    {
        Task<AuthorizationResponse> AuthenticateAsync(PasswordAuthorizeRequest userData, CancellationToken cancellationToken);
    }
}