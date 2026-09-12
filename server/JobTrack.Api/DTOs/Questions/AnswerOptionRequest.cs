using System.ComponentModel.DataAnnotations;

namespace JobTrack.Api.DTOs.Questions;

public class AnswerOptionRequest
{
    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}