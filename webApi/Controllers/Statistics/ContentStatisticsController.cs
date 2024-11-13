using System.ComponentModel.DataAnnotations;
using API.Controllers.Base;
using Common.Dto.Statistics;
using Common.Models.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Abstract.Statistics;

namespace API.Controllers.Statistics;

[Route("api/stats")]
public class ContentStatisticsController : AppBaseController
{
    private readonly IContentStatisticsService _service;

    public ContentStatisticsController(IContentStatisticsService service)
    {
        _service = service;
    }

    [HttpPost("post/{postId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostStats(
        [FromRoute] [Required] int postId,
        [FromQuery] [Required] TimeMeasure measure,
        [FromBody] [Required] GetStatsForPostDto dto,
        CancellationToken ct)
    {
        var model = await _service.GetForPost(dto with { PostId = postId }, measure, ct);
        return Ok(model);
    }
}
