using Common.Dto.Statistics;
using Common.Exceptions;
using Common.Models.Statistics;
using DAL.Repositories.Abstract;
using Service.Abstract.Statistics;

namespace Service.Statistics;

public class ContentStatisticsService : IContentStatisticsService
{
    private readonly IPostRepository _postRepository;

    public ContentStatisticsService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }
    public async Task<PostActivityModel> GetForPost(GetStatsForPostDto dto, TimeMeasure measure, CancellationToken ct = default)
    {
        if (dto.StartDate > dto.EndDate)
        {
            throw new ValidationException($"StartDate is greater than EndDate");
        }

        var postExists = await _postRepository.ExistsAsync(dto.PostId, ct);

        if (!postExists)
        {
            throw new NotFoundException($"Post with ID {dto.PostId} was not found");
        }

        var dates = new List<DateTime>();

        if (measure != TimeMeasure.Day)
        {
            dto.StartDate = new DateTime(dto.StartDate.Year, dto.StartDate.Month, 1);
            dto.EndDate = new DateTime(dto.EndDate.Year, dto.EndDate.Month, 1);
        }

        for (var date = dto.StartDate; date <= dto.EndDate; date = measure switch { TimeMeasure.Day => date.AddDays(1), TimeMeasure.Month => date.AddMonths(1) })
        {
            dates.Add(date.Date);
        }

        var result = await _postRepository.GetPostActivity(dto.PostId, dates, measure, ct);
        return result;
    }
}