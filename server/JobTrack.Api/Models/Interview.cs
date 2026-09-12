namespace JobTrack.Api.Models;

public class Interview
{
    public int Id { get; set; }

    public DateTime InterviewDate { get; set; }

    public InterviewType Type { get; set; }

    public string? ContactPerson { get; set; }

    public string? Notes { get; set; }

    public InterviewOutcome? Outcome { get; set; }

    public int JobApplicationId { get; set; }

    public JobApplication JobApplication { get; set; } = null!;
}