namespace JobTrack.Api.Models;

public class TestQuestion
{
    public int TestId { get; set; }

    public Test Test { get; set; } = null!;

    public int QuestionId { get; set; }

    public Question Question { get; set; } = null!;

    public int OrderNumber { get; set; }
}