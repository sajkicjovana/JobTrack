using System.ComponentModel.DataAnnotations;

namespace JobTrack.Api.DTOs.Questions;

public class UpdateQuestionRequest
{
    [Required]
    [MaxLength(1000)]
    public string Text { get; set; } = string.Empty;

    public int TopicId { get; set; }

    [MinLength(2)]
    public List<AnswerOptionRequest> Answers { get; set; }
        = new List<AnswerOptionRequest>();
}