using Common.Dto.Statistics;
using Common.Models.Statistics;

namespace Service.Abstract.Statistics;

public interface IContentStatisticsService
{
    Task<PostActivityModel> GetForPost(
        GetStatsForPostDto dto,
        TimeMeasure measure,
        CancellationToken ct = default);
}