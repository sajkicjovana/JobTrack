namespace JobTrack.Api.DTOs.TestAttempts;

public class SubmitAnswerRequest
{
    public int QuestionId { get; set; }

    public int AnswerOptionId { get; set; }
}