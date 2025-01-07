using Common.Dto.Auth;
using DAL.Repositories.Abstract;
using Service.Abstract.Auth;
using Common.Exceptions;

namespace Service.Auth
{
    public class PasswordAuthService : IPasswordAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IAuthorizationService _authorizationService;

        public PasswordAuthService(
            IUserRepository userRepository,
            IEncryptionService encryptionService,
            IAuthorizationService authorizationService)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
            _authorizationService = authorizationService;
        }

        public async Task<AuthorizationResponse> AuthenticateAsync(PasswordAuthorizeRequest userData,
            CancellationToken cancellationToken)
        {
            var hashedPassword = _encryptionService.EncryptPassword(userData.Password);
            var user = await _userRepository.GetUserByCredentials(userData.Username, hashedPassword, cancellationToken)
                       ?? throw new ValidationException("Credentials were not valid");

            return await _authorizationService.Authorize(user, cancellationToken);
        }
    }
}