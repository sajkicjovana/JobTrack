namespace JobTrack.Api.Models;

public class TestAnswer
{
    public int Id { get; set; }

    public int TestAttemptId { get; set; }

    public TestAttempt TestAttempt { get; set; } = null!;

    public int QuestionId { get; set; }

    public Question Question { get; set; } = null!;

    public int? SelectedAnswerOptionId { get; set; }

    public AnswerOption? SelectedAnswerOption { get; set; }

    public bool IsCorrect { get; set; }
}