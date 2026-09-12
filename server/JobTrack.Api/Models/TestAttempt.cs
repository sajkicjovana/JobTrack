namespace JobTrack.Api.Models;

public class TestAttempt
{
    public int Id { get; set; }

    public DateTime StartedAt { get; set; }
        = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public int CorrectAnswers { get; set; }

    public int TotalQuestions { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public int TestId { get; set; }

    public Test Test { get; set; } = null!;

    public ICollection<TestAnswer> Answers { get; set; }
        = new List<TestAnswer>();
}