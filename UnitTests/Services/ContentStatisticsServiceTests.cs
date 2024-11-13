using Common.Dto.Statistics;
using Common.Exceptions;
using Common.Models.Statistics;
using DAL.Repositories.Abstract;
using Service.Statistics;

public class ContentStatisticsServiceTests
{
    private readonly IPostRepository _postRepository;
    private readonly ContentStatisticsService _service;
    private const int _postId = 200;

    public ContentStatisticsServiceTests()
    {
        _postRepository = Substitute.For<IPostRepository>();
        _service = new ContentStatisticsService(_postRepository);
    }

    [Fact]
    public async Task GetForPost_ShouldCalculateDatesList_ForDaysMeasure()
    {
        // Arrange
        var dto = new GetStatsForPostDto
        {
            PostId = _postId,
            StartDate = new DateTime(2023, 1, 1),
            EndDate = new DateTime(2023, 1, 5)
        };

        var expectedDates = new List<DateTime>
        {
            new(2023, 1, 1),
            new(2023, 1, 2),
            new(2023, 1, 3),
            new (2023, 1, 4),
            new(2023, 1, 5)
        };

        _postRepository.ExistsAsync(_postId, Arg.Any<CancellationToken>()).Returns(true);
        _postRepository.GetPostActivity(_postId, expectedDates, TimeMeasure.Day, Arg.Any<CancellationToken>()).Returns(new PostActivityModel());

        // Act
        await _service.GetForPost(dto, TimeMeasure.Day);

        // Assert
        await _postRepository.Received(1).GetPostActivity(
            _postId,
            Arg.Is<List<DateTime>>(dates => dates.Count == expectedDates.Count && dates.All(d => expectedDates.Contains(d))),
            TimeMeasure.Day,
            CancellationToken.None);
    }

    [Fact]
    public async Task GetForPost_ShouldCalculateDatesList_ForMonthsMeasure()
    {
        // Arrange
        var dto = new GetStatsForPostDto
        {
            PostId = _postId,
            StartDate = new DateTime(2023, 1, 1),
            EndDate = new DateTime(2023, 4, 1)
        };

        var expectedDates = new List<DateTime>
        {
            new(2023, 1, 1),
            new(2023, 2, 1),
            new(2023, 3, 1),
            new (2023, 4, 1),
        };

        _postRepository.ExistsAsync(_postId, Arg.Any<CancellationToken>()).Returns(true);
        _postRepository.GetPostActivity(_postId, expectedDates, TimeMeasure.Month, Arg.Any<CancellationToken>()).Returns(new PostActivityModel());

        // Act
        await _service.GetForPost(dto, TimeMeasure.Month);

        // Assert
        await _postRepository.Received(1).GetPostActivity(
            _postId,
            Arg.Is<List<DateTime>>(dates => dates.Count == expectedDates.Count && dates.All(d => expectedDates.Contains(d))),
            TimeMeasure.Month,
            CancellationToken.None);
    }

    [Fact]
    public async Task GetForPost_ShouldCalculateDatesList_ForMonth_FloorsDays()
    {
        // Arrange
        var dto = new GetStatsForPostDto
        {
            PostId = _postId,
            StartDate = new DateTime(2023, 1, 29),
            EndDate = new DateTime(2023, 6, 15)
        };

        var expectedDates = new List<DateTime>
        {
            new(2023, 1, 1),
            new(2023, 2, 1),
            new(2023, 3, 1),
            new (2023, 4, 1),
            new (2023, 5, 1),
            new (2023, 6, 1),

        };

        _postRepository.ExistsAsync(_postId, Arg.Any<CancellationToken>()).Returns(true);
        _postRepository.GetPostActivity(_postId, expectedDates, TimeMeasure.Month, Arg.Any<CancellationToken>()).Returns(new PostActivityModel());

        // Act
        await _service.GetForPost(dto, TimeMeasure.Month);

        // Assert
        await _postRepository.Received(1).GetPostActivity(
            _postId,
            Arg.Is<List<DateTime>>(dates => dates.Count == expectedDates.Count && dates.All(d => expectedDates.Contains(d))),
            TimeMeasure.Month,
            CancellationToken.None);
    }

    [Fact]
    public async Task GetForPost_ShouldThrowNotFoundException_WhenPostDoesNotExist()
    {
        // Arrange
        var dto = new GetStatsForPostDto
        {
            PostId = 200,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(5)
        };

        _postRepository.ExistsAsync(dto.PostId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var act = async () => await _service.GetForPost(dto, TimeMeasure.Day);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Post with ID {dto.PostId} was not found");
    }

}
