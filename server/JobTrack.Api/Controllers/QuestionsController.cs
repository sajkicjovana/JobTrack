using JobTrack.Api.Data;
using JobTrack.Api.DTOs.Questions;
using JobTrack.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class QuestionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public QuestionsController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll(
        int? topicId)
    {
        var query = _context.Questions.AsQueryable();

        if (topicId.HasValue)
        {
            query = query.Where(q =>
                q.TopicId == topicId.Value
            );
        }

        var questions = await query
            .OrderBy(q => q.Topic.Technology.Name)
            .ThenBy(q => q.Topic.Name)
            .Select(q => new
            {
                q.Id,
                q.Text,
                q.TopicId,

                TopicName =
                    q.Topic.Name,

                TechnologyId =
                    q.Topic.TechnologyId,

                TechnologyName =
                    q.Topic.Technology.Name,

                Answers = q.AnswerOptions
                    .Select(a => new
                    {
                        a.Id,
                        a.Text,
                        a.IsCorrect
                    })
                    .ToList()
            })
            .ToListAsync();

        return Ok(questions);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var question = await _context.Questions
            .Where(q => q.Id == id)
            .Select(q => new
            {
                q.Id,
                q.Text,
                q.TopicId,

                TopicName =
                    q.Topic.Name,

                TechnologyId =
                    q.Topic.TechnologyId,

                TechnologyName =
                    q.Topic.Technology.Name,

                Answers = q.AnswerOptions
                    .Select(a => new
                    {
                        a.Id,
                        a.Text,
                        a.IsCorrect
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (question == null)
        {
            return NotFound(
                "Question not found."
            );
        }

        return Ok(question);
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        CreateQuestionRequest request)
    {
        var topic =
            await _context.Topics
                .FindAsync(request.TopicId);

        if (topic == null)
        {
            return BadRequest(
                "Topic not found."
            );
        }

        if (request.Answers.Count < 2)
        {
            return BadRequest(
                "Question must have at least two answers."
            );
        }

        var correctAnswers =
            request.Answers.Count(a => a.IsCorrect);

        if (correctAnswers != 1)
        {
            return BadRequest(
                "Question must have exactly one correct answer."
            );
        }

        var question = new Question
        {
            Text = request.Text.Trim(),
            TopicId = request.TopicId
        };

        foreach (var answer in request.Answers)
        {
            question.AnswerOptions.Add(
                new AnswerOption
                {
                    Text = answer.Text.Trim(),
                    IsCorrect = answer.IsCorrect
                }
            );
        }

        _context.Questions.Add(question);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Question created successfully.",
            id = question.Id
        });
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateQuestionRequest request)
    {
        var question =
            await _context.Questions
                .Include(q => q.AnswerOptions)
                .FirstOrDefaultAsync(q =>
                    q.Id == id
                );

        if (question == null)
        {
            return NotFound(
                "Question not found."
            );
        }

        var topic =
            await _context.Topics
                .FindAsync(request.TopicId);

        if (topic == null)
        {
            return BadRequest(
                "Topic not found."
            );
        }

        if (request.Answers.Count < 2)
        {
            return BadRequest(
                "Question must have at least two answers."
            );
        }

        var correctAnswers =
            request.Answers.Count(a => a.IsCorrect);

        if (correctAnswers != 1)
        {
            return BadRequest(
                "Question must have exactly one correct answer."
            );
        }

        question.Text = request.Text.Trim();
        question.TopicId = request.TopicId;

        _context.AnswerOptions.RemoveRange(
            question.AnswerOptions
        );

        question.AnswerOptions.Clear();

        foreach (var answer in request.Answers)
        {
            question.AnswerOptions.Add(
                new AnswerOption
                {
                    Text = answer.Text.Trim(),
                    IsCorrect = answer.IsCorrect
                }
            );
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Question updated successfully."
        });
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var question =
            await _context.Questions
                .FirstOrDefaultAsync(q =>
                    q.Id == id
                );

        if (question == null)
        {
            return NotFound(
                "Question not found."
            );
        }

        var usedInTest =
            await _context.TestQuestions
                .AnyAsync(tq =>
                    tq.QuestionId == id
                );

        if (usedInTest)
        {
            return BadRequest(
                "Question is used in a test and cannot be deleted."
            );
        }

        _context.Questions.Remove(question);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Question deleted successfully."
        });
    }
}