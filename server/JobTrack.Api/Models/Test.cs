namespace JobTrack.Api.Models;

public class Test
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsPublished { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int TechnologyId { get; set; }

    public Technology Technology { get; set; } = null!;

    public ICollection<TestQuestion> TestQuestions { get; set; }
        = new List<TestQuestion>();

    public ICollection<TestAttempt> TestAttempts { get; set; }
    = new List<TestAttempt>();
}