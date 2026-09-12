namespace JobTrack.Api.Models;

public class JobApplication
{
    public int Id { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string Position { get; set; } = string.Empty;

    public string? JobUrl { get; set; }

    public string? Location { get; set; }

    public string? Notes { get; set; }

    public JobApplicationStatus Status { get; set; }
    = JobApplicationStatus.Saved;

    public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<JobApplicationTechnology> JobApplicationTechnologies { get; set; }
    = new List<JobApplicationTechnology>();

    public ICollection<Interview> Interviews { get; set; }
    = new List<Interview>();
}