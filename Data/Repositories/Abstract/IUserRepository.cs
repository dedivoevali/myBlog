using DAL.Repositories.Abstract.Base;
using Domain;

namespace DAL.Repositories.Abstract;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetProfileData(int userId, CancellationToken ct = default);
    Task<bool> IsBanned(int userId, CancellationToken ct = default);
    Task<bool> IsNicknameOccupied(string username, CancellationToken ct = default);
}