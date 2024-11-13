using Common.Models.Statistics;
using DAL.Repositories.Abstract;
using DAL.Repositories.Abstract.Base;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class PostRepository : BaseRepository<Post>, IPostRepository
    {
        public PostRepository(BlogDbContext db) : base(db)
        {
        }

        public async Task<Post?> GetByTitleAsync(string title, CancellationToken cancellationToken)
        {
            var found = await _db.Posts.Where(e => e.Title == title).FirstOrDefaultAsync(cancellationToken);

            return found;
        }

        public async Task<PostActivityModel> GetPostActivity(int postId, IEnumerable<DateTime> dates, TimeMeasure measure, CancellationToken ct)
        {
            switch (measure)
            {
                case TimeMeasure.Day:
                {
                    var comments = await _db.Comments
                        .Where(c => dates.Contains(c.RegistrationDate.Date) && c.PostId == postId)
                        .GroupBy(c => c.RegistrationDate.Date)
                        .Select(group => new
                        {
                            StartDate = group.Key,
                            Comments = group.Count()
                        }).ToListAsync(ct);

                    var reactions = await _db.PostReactions
                        .Where(pr => dates.Contains(pr.RegistrationDate.Date) && pr.PostId == postId)
                        .GroupBy(pr => pr.RegistrationDate.Date)
                        .Select(group => new
                        {
                            StartDate = group.Key,
                            Reactions = group.Count()
                        }).ToListAsync(ct);

                    return new PostActivityModel(postId, measure)
                    {
                        Steps = dates.Select(d => new PostActivityStepModel
                        {
                            Reactions = reactions.FirstOrDefault(r => r.StartDate == d)?.Reactions ?? 0,
                            Comments = comments.FirstOrDefault(r => r.StartDate == d)?.Comments ?? 0,
                            StartDate = DateOnly.FromDateTime(d)
                        })
                    };
                }
                case TimeMeasure.Month:
                {
                    var comments = await _db.Comments
                        .Where(c =>
                            c.PostId == postId &&
                            dates.Any(d =>
                                d.Month == c.RegistrationDate.Month &&
                                d.Year == c.RegistrationDate.Year))
                        .GroupBy(c => new { c.RegistrationDate.Year, c.RegistrationDate.Month })
                        .Select(group => new
                        {
                            StartDate = group.Key,
                            Comments = group.Count()
                        }).ToListAsync(ct);

                    var reactions = await _db.PostReactions
                        .Where(pr => pr.PostId == postId &&
                                     dates.Any(d =>
                                         d.Month == pr.RegistrationDate.Month &&
                                         d.Year == pr.RegistrationDate.Year))
                        .GroupBy(pr => new { pr.RegistrationDate.Year, pr.RegistrationDate.Month })
                        .Select(group => new
                        {
                            StartDate = group.Key,
                            Reactions = group.Count()
                        }).ToListAsync(ct);

                    return new PostActivityModel(postId, measure)
                    {
                        Steps = dates.Select(d => new PostActivityStepModel
                        {
                            Reactions = reactions.FirstOrDefault(r => r.StartDate.Month == d.Month && r.StartDate.Year == d.Year)?.Reactions ?? 0,
                            Comments = comments.FirstOrDefault(c => c.StartDate.Month == d.Month && c.StartDate.Year == d.Year)?.Comments ?? 0,
                            StartDate = DateOnly.FromDateTime(d)
                        })
                    };
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(measure), measure, null);
            }
        }
    }
}