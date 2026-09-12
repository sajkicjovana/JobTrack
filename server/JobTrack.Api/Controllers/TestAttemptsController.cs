using System.Security.Claims;
using JobTrack.Api.Data;
using JobTrack.Api.DTOs.TestAttempts;
using JobTrack.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TestAttemptsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TestAttemptsController(
        AppDbContext context)
    {
        _context = context;
    }


    [HttpPost("start/{testId}")]
    public async Task<IActionResult> StartTest(
        int testId)
    {
        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim);


        var test = await _context.Tests
            .Where(t =>
                t.Id == testId &&
                t.IsPublished
            )
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.Description,

                TechnologyName =
                    t.Technology.Name,

                Questions =
                    t.TestQuestions
                        .OrderBy(tq =>
                            tq.OrderNumber
                        )
                        .Select(tq => new
                        {
                            tq.Question.Id,
                            tq.Question.Text,

                            TopicName =
                                tq.Question.Topic.Name,

                            Answers =
                                tq.Question.AnswerOptions
                                    .Select(a => new
                                    {
                                        a.Id,
                                        a.Text
                                    })
                                    .ToList()
                        })
                        .ToList()
            })
            .FirstOrDefaultAsync();


        if (test == null)
        {
            return NotFound(
                "Published test not found."
            );
        }


        var attempt = new TestAttempt
        {
            UserId = userId,
            TestId = test.Id,
            StartedAt = DateTime.UtcNow,
            TotalQuestions =
                test.Questions.Count
        };


        _context.TestAttempts.Add(attempt);

        await _context.SaveChangesAsync();


        return Ok(new
        {
            attemptId = attempt.Id,

            test.Id,
            test.Title,
            test.Description,
            test.TechnologyName,
            test.Questions
        });
    }


    [HttpPost("{attemptId}/submit")]
    public async Task<IActionResult> SubmitTest(
        int attemptId,
        SubmitTestRequest request)
    {
        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim);


        var attempt =
            await _context.TestAttempts
                .Include(a => a.Test)
                    .ThenInclude(t =>
                        t.TestQuestions
                    )
                        .ThenInclude(tq =>
                            tq.Question
                        )
                            .ThenInclude(q =>
                                q.AnswerOptions
                            )
                .FirstOrDefaultAsync(a =>
                    a.Id == attemptId &&
                    a.UserId == userId
                );


        if (attempt == null)
        {
            return NotFound(
                "Test attempt not found."
            );
        }


        if (attempt.CompletedAt != null)
        {
            return BadRequest(
                "This test attempt has already been submitted."
            );
        }


        var submittedAnswers =
            request.Answers
                .GroupBy(a => a.QuestionId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Last()
                );


        var correctAnswers = 0;


        foreach (
            var testQuestion
            in attempt.Test.TestQuestions
                .OrderBy(tq =>
                    tq.OrderNumber
                )
        )
        {
            var question =
                testQuestion.Question;


            int? selectedAnswerId = null;

            var isCorrect = false;


            if (
                submittedAnswers.TryGetValue(
                    question.Id,
                    out var submittedAnswer
                )
            )
            {
                var selectedAnswer =
                    question.AnswerOptions
                        .FirstOrDefault(a =>
                            a.Id ==
                            submittedAnswer.AnswerOptionId
                        );


                if (selectedAnswer == null)
                {
                    return BadRequest(
                        $"Invalid answer for question {question.Id}."
                    );
                }


                selectedAnswerId =
                    selectedAnswer.Id;

                isCorrect =
                    selectedAnswer.IsCorrect;
            }


            if (isCorrect)
            {
                correctAnswers++;
            }


            attempt.Answers.Add(
                new TestAnswer
                {
                    QuestionId =
                        question.Id,

                    SelectedAnswerOptionId =
                        selectedAnswerId,

                    IsCorrect =
                        isCorrect
                }
            );
        }


        attempt.CorrectAnswers =
            correctAnswers;

        attempt.TotalQuestions =
            attempt.Test.TestQuestions.Count;

        attempt.CompletedAt =
            DateTime.UtcNow;


        await _context.SaveChangesAsync();


        var percentage =
            attempt.TotalQuestions == 0
                ? 0
                : Math.Round(
                    (double)attempt.CorrectAnswers /
                    attempt.TotalQuestions * 100,
                    2
                );


        return Ok(new
        {
            attemptId = attempt.Id,

            correctAnswers =
                attempt.CorrectAnswers,

            totalQuestions =
                attempt.TotalQuestions,

            percentage
        });
    }


    [HttpGet("my-results")]
    public async Task<IActionResult> GetMyResults()
    {
        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim);


        var results =
            await _context.TestAttempts
                .Where(a =>
                    a.UserId == userId &&
                    a.CompletedAt != null
                )
                .OrderByDescending(a =>
                    a.CompletedAt
                )
                .Select(a => new
                {
                    a.Id,

                    TestId =
                        a.TestId,

                    TestTitle =
                        a.Test.Title,

                    TechnologyId =
                        a.Test.TechnologyId,

                    TechnologyName =
                        a.Test.Technology.Name,

                    a.CorrectAnswers,
                    a.TotalQuestions,
                    a.CompletedAt,

                    Percentage =
                        a.TotalQuestions == 0
                            ? 0
                            : (double)a.CorrectAnswers /
                              a.TotalQuestions * 100
                })
                .ToListAsync();


        return Ok(results);
    }


    [HttpGet("{attemptId}/result")]
    public async Task<IActionResult> GetResult(
        int attemptId)
    {
        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim);


        var attempt =
            await _context.TestAttempts
                .Where(a =>
                    a.Id == attemptId &&
                    a.UserId == userId &&
                    a.CompletedAt != null
                )
                .Select(a => new
                {
                    a.Id,

                    TestTitle =
                        a.Test.Title,

                    TechnologyName =
                        a.Test.Technology.Name,

                    a.CorrectAnswers,
                    a.TotalQuestions,
                    a.CompletedAt,

                    Answers =
                        a.Answers
                            .Select(answer => new
                            {
                                answer.QuestionId,

                                QuestionText =
                                    answer.Question.Text,

                                TopicName =
                                    answer.Question.Topic.Name,

                                answer.IsCorrect,

                                SelectedAnswer =
                                    answer.SelectedAnswerOption != null
                                        ? answer.SelectedAnswerOption.Text
                                        : null,

                                CorrectAnswer =
                                    answer.Question.AnswerOptions
                                        .Where(option =>
                                            option.IsCorrect
                                        )
                                        .Select(option =>
                                            option.Text
                                        )
                                        .FirstOrDefault()
                            })
                            .ToList()
                })
                .FirstOrDefaultAsync();


        if (attempt == null)
        {
            return NotFound(
                "Result not found."
            );
        }


        return Ok(attempt);
    }
}