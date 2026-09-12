using System.ComponentModel.DataAnnotations;

namespace JobTrack.Api.DTOs.Tests;

public class CreateTestRequest
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public int TechnologyId { get; set; }

    [MinLength(1)]
    public List<int> QuestionIds { get; set; }
        = new List<int>();
}