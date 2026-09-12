namespace JobTrack.Api.Models;

public class Question
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public int TopicId { get; set; }

    public Topic Topic { get; set; } = null!;

    public ICollection<AnswerOption> AnswerOptions { get; set; }
        = new List<AnswerOption>();

    public ICollection<TestQuestion> TestQuestions { get; set; }
        = new List<TestQuestion>();
    
    public ICollection<TestAnswer> TestAnswers { get; set; }
    = new List<TestAnswer>();
}