using Common.Exceptions;
using DAL.Repositories.Abstract;
using Domain;
using Service.Abstract;
using System.Linq.Expressions;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> Add(User entity, CancellationToken cancellationToken)
        {
            return await _userRepository.AddAsync(entity, cancellationToken);
        }

        public async Task<User> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);

            if (user == null)
            {
                throw new ValidationException($"{nameof(User)} of ID: {id} does not exist");
            }

            return user;
        }

        public async Task RemoveAsync(int id, int issuerId, CancellationToken cancellationToken)
        {
            if (id != issuerId)
            {
                throw new InsufficientPermissionsException(
                    $"{nameof(User)} of ID : {issuerId} cannot delete this account!");
            }

            await _userRepository.RemoveAsync(id, cancellationToken);
        }

        public async Task<User> UpdateAsync(User request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken) ??
                       throw new ValidationException($"User with ID {request.Id} was not found");

            if (!string.IsNullOrEmpty(request.Username))
            {
                var newUsernameDuplicates =
                    await _userRepository.GetWhereAsync(user => user.Username == request.Username, cancellationToken);
                if (newUsernameDuplicates.Any())
                {
                    throw new ValidationException($"Username {request.Username} is occupied");
                }

                user.Username = request.Username;
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            return await _userRepository.Update(user, cancellationToken);
        }

        public async Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            var usernameFound = await _userRepository.GetWhereAsync(e => e.Username == username, cancellationToken);

            return usernameFound.FirstOrDefault() ??
                   throw new ValidationException($"{nameof(User)} with name {username} was not found!");
        }


        public async Task<User> GetByIdWithIncludeAsync(int id, CancellationToken cancellationToken,
            params Expression<Func<User, object>>[] includeProperties)
        {
            var user = await _userRepository.GetByIdWithIncludeAsync(id, cancellationToken, includeProperties);

            return user ?? throw new ValidationException($"{nameof(User)} of ID: {id} does not exist");
        }

        public async Task UpdateLastActivity(int userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
                ?? throw new ValidationException($"{nameof(User)} of ID: {userId} does not exist");

            user.LastActivity = DateTime.UtcNow;

            await _userRepository.Update(user, cancellationToken);
        }

        public async Task<IEnumerable<Passkey>> GetPasskeys(int userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdWithIncludeAsync(userId,
                cancellationToken, 
                e => e.Passkeys)
                 ?? throw new NotFoundException($"{nameof(User)} of ID: {userId} does not exist");

            return user.Passkeys;
        }

        public async Task<User?> GetUserProfileData(int userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetProfileData(userId, cancellationToken)
                       ?? throw new NotFoundException($"{nameof(User)} of ID: {userId} does not exist");

            return user;
        }
    }
}