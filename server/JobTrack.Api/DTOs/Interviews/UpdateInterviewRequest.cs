using System.ComponentModel.DataAnnotations;
using JobTrack.Api.Models;

namespace JobTrack.Api.DTOs.Interviews;

public class UpdateInterviewRequest
{
    public int JobApplicationId { get; set; }

    public DateTime InterviewDate { get; set; }

    public InterviewType Type { get; set; }

    [MaxLength(100)]
    public string? ContactPerson { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public InterviewOutcome? Outcome { get; set; }
}