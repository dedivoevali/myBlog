using Microsoft.AspNetCore.Http;

namespace Service.Abstract
{
    public interface IAvatarService
    {
        Task<string> GetAvatarUrlAsync(int userId, CancellationToken cancellationToken);

        Task<string> AddAndGetUrlAsync(IFormFile file, int userId, CancellationToken cancellationToken);

        Task RemoveAsync(int issuerId, CancellationToken cancellationToken);
    }
}