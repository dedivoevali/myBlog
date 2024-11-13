using System.ComponentModel.DataAnnotations;

namespace Common.Dto.Statistics;

public record GetStatsForPostDto
{
    public int PostId { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
}