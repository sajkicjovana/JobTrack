namespace JobTrack.Api.Models;

public class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "User";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<JobApplication> JobApplications { get; set; }
    = new List<JobApplication>();
    
    public ICollection<TestAttempt> TestAttempts { get; set; }
    = new List<TestAttempt>();
}