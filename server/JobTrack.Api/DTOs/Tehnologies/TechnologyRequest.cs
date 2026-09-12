using System.ComponentModel.DataAnnotations;

namespace JobTrack.Api.DTOs.Technologies;

public class TechnologyRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}