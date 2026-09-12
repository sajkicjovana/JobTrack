using System.ComponentModel.DataAnnotations;

namespace JobTrack.Api.DTOs.Topics;

public class TopicRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public int TechnologyId { get; set; }
}