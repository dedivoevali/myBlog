using AutoMapper;
using Common.Dto.Auth;
using Common.Exceptions;
using DAL.Repositories.Abstract;
using Domain;
using Domain.Abstract;
using Service.Abstract.Auth;

namespace Service.Auth
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEncryptionService _encryptionService;
        private readonly IUnitOfWork _unitOfWork;

        public RegistrationService(IUserRepository userRepository,
            IMapper mapper,
            IEncryptionService encryptionService,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _mapper = mapper;
            _encryptionService = encryptionService;
        }

        public async Task<User> RegisterAsync(RegistrationDto registerData, CancellationToken cancellationToken)
        {
            if (await _userRepository.IsNicknameOccupied(registerData.Username, cancellationToken))
            {
                throw new ValidationException($"Username {registerData.Username} is occupied");
            }

            if (PasswordsDoNotMatch(registerData.Password, registerData.ConfirmPassword))
            {
                throw new ValidationException($"Passwords do not match");
            }

            var newUserEntity = _mapper.Map<User>(registerData);
            newUserEntity.PasswordHash = _encryptionService.EncryptPassword(newUserEntity.Password);

            newUserEntity = await _userRepository.AddAsync(newUserEntity, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return newUserEntity;
        }

        private static bool PasswordsDoNotMatch(string actualPassword, string confirmationPassword)
        {
            return string.CompareOrdinal(actualPassword, confirmationPassword) != 0;
        }
    }
}