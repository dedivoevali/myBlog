using DAL.Repositories.Abstract;
using DAL.Repositories.Abstract.Base;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(BlogDbContext db) : base(db)
    {
    }

    public async Task<User?> GetProfileData(int userId, CancellationToken ct = default)
    {
        return await _db.Users
            .Include(e => e.Avatar)
            .Include(e => e.Warnings.Where(e => e.RemovedAt == null))
            .FirstOrDefaultAsync(e => e.Id == userId, ct);
    }

    public async Task<bool> IsBanned(int userId, CancellationToken ct = default)
    {
        return await _db.Users.Where(u => u.Id == userId && u.IsBanned).CountAsync(ct) > 0;
    }

    public async Task<bool> IsNicknameOccupied(string username, CancellationToken ct = default)
    {
        return await _db.Users.AnyAsync(u => u.Username == username, ct);
    }

    public async Task<User?> GetUserByCredentials(string username, string passwordHash, CancellationToken ct = default)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == passwordHash, ct);
    }

    public async Task<User?> GetUserByActiveRefreshToken(string refreshToken, CancellationToken ct = default)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiresAt > DateTime.UtcNow, ct);
    }
}