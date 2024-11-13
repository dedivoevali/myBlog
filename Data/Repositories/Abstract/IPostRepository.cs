using Common.Models.Statistics;
using DAL.Repositories.Abstract.Base;
using Domain;

namespace DAL.Repositories.Abstract
{
    public interface IPostRepository : IBaseRepository<Post>
    {
        Task<Post?> GetByTitleAsync(string title, CancellationToken cancellationToken);
        Task<PostActivityModel> GetPostActivity(
            int postId,
            IEnumerable<DateTime> dates,
            TimeMeasure measure,
            CancellationToken ct);
    }
}