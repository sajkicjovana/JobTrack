using JobTrack.Api.Models;
using System.ComponentModel.DataAnnotations;
namespace JobTrack.Api.DTOs.JobApplications;

public class CreateJobApplicationRequest
{
    [Required]
    [MaxLength(100)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Position { get; set; } = string.Empty;

    [Url]
    public string? JobUrl { get; set; }

    [MaxLength(1000)]
    public string? Location { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public JobApplicationStatus Status { get; set; }
    = JobApplicationStatus.Saved;

    public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;

    public List<int> TechnologyIds { get; set; }
    = new List<int>();
}