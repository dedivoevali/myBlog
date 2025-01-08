using Common.Models;
using Domain;

namespace Service.Abstract
{
    public interface IUserService : IEntityService<User>
    {
        Task UpdateLastActivity(int userId, CancellationToken cancellationToken);
        Task<IEnumerable<Passkey>> GetPasskeys(int userId, CancellationToken cancellationToken);
        Task<User?> GetUserProfileData(int userId, CancellationToken cancellationToken);
        Task<UserBadgeModel> GetBadge(int userId, CancellationToken cancellationToken);
    }
}