namespace JobTrack.Api.DTOs.TestAttempts;

public class SubmitTestRequest
{
    public List<SubmitAnswerRequest> Answers { get; set; }
        = new List<SubmitAnswerRequest>();
}