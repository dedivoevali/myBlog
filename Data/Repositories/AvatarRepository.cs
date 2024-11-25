using DAL.Repositories.Abstract;
using DAL.Repositories.Abstract.Base;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class AvatarRepository(BlogDbContext db) : BaseRepository<Avatar>(db), IAvatarRepository
{
    public async Task<Avatar?> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        var avatarInfo = await _db.Avatars.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

        return avatarInfo;
    }

    public async Task<bool> HasAvatar(int userId, CancellationToken ct)
    {
        return await _db.Avatars.AnyAsync(a => a.UserId == userId, ct);
    }
}
