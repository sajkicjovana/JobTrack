using JobTrack.Api.Data;
using JobTrack.Api.DTOs.Tests;
using JobTrack.Api.Models;
using JobTrack.Api.Hubs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace JobTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TestsController : ControllerBase
{
    private readonly AppDbContext _context;

    private readonly IHubContext<NotificationHub>
        _notificationHub;


    public TestsController(
        AppDbContext context,
        IHubContext<NotificationHub> notificationHub)
    {
        _context = context;
        _notificationHub = notificationHub;
    }


    // USER + ADMIN
    // Vraća samo objavljene testove.

    [HttpGet("published")]
    public async Task<IActionResult> GetPublished()
    {
        var tests = await _context.Tests
            .Where(t => t.IsPublished)
            .OrderBy(t => t.Technology.Name)
            .ThenBy(t => t.Title)
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.Description,
                t.TechnologyId,

                TechnologyName =
                    t.Technology.Name,

                QuestionCount =
                    t.TestQuestions.Count
            })
            .ToListAsync();

        return Ok(tests);
    }


    // ADMIN
    // Pregled svih testova, uključujući draft.

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAdmin()
    {
        var tests = await _context.Tests
            .OrderBy(t => t.Technology.Name)
            .ThenBy(t => t.Title)
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.Description,
                t.IsPublished,
                t.CreatedAt,
                t.TechnologyId,

                TechnologyName =
                    t.Technology.Name,

                QuestionCount =
                    t.TestQuestions.Count
            })
            .ToListAsync();

        return Ok(tests);
    }


    [HttpGet("admin/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAdminById(
        int id)
    {
        var test = await _context.Tests
            .Where(t => t.Id == id)
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.Description,
                t.IsPublished,
                t.CreatedAt,
                t.TechnologyId,

                TechnologyName =
                    t.Technology.Name,

                Questions =
                    t.TestQuestions
                        .OrderBy(tq =>
                            tq.OrderNumber
                        )
                        .Select(tq => new
                        {
                            tq.OrderNumber,

                            tq.Question.Id,
                            tq.Question.Text,

                            TopicId =
                                tq.Question.TopicId,

                            TopicName =
                                tq.Question.Topic.Name,

                            Answers =
                                tq.Question.AnswerOptions
                                    .Select(a => new
                                    {
                                        a.Id,
                                        a.Text,
                                        a.IsCorrect
                                    })
                                    .ToList()
                        })
                        .ToList()
            })
            .FirstOrDefaultAsync();

        if (test == null)
        {
            return NotFound(
                "Test not found."
            );
        }

        return Ok(test);
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        CreateTestRequest request)
    {
        var technology =
            await _context.Technologies
                .FindAsync(request.TechnologyId);

        if (technology == null)
        {
            return BadRequest(
                "Technology not found."
            );
        }

        var questionIds =
            request.QuestionIds
                .Distinct()
                .ToList();

        if (questionIds.Count == 0)
        {
            return BadRequest(
                "Test must contain at least one question."
            );
        }

        var questions =
            await _context.Questions
                .Where(q =>
                    questionIds.Contains(q.Id)
                )
                .Select(q => new
                {
                    q.Id,

                    TechnologyId =
                        q.Topic.TechnologyId
                })
                .ToListAsync();

        if (questions.Count != questionIds.Count)
        {
            return BadRequest(
                "One or more questions do not exist."
            );
        }

        if (questions.Any(q =>
            q.TechnologyId != request.TechnologyId))
        {
            return BadRequest(
                "All questions must belong to the selected technology."
            );
        }

        var test = new Test
        {
            Title = request.Title.Trim(),
            Description = request.Description,
            TechnologyId = request.TechnologyId,
            IsPublished = false
        };

        for (var i = 0; i < questionIds.Count; i++)
        {
            test.TestQuestions.Add(
                new TestQuestion
                {
                    QuestionId = questionIds[i],
                    OrderNumber = i + 1
                }
            );
        }

        _context.Tests.Add(test);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message =
                "Test created successfully.",

            id = test.Id
        });
    }


    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int id,
        UpdateTestRequest request)
    {
        var test =
            await _context.Tests
                .Include(t =>
                    t.TestQuestions
                )
                .FirstOrDefaultAsync(t =>
                    t.Id == id
                );

        if (test == null)
        {
            return NotFound(
                "Test not found."
            );
        }


        var hasAttempts =
            await _context.TestAttempts
                .AnyAsync(a =>
                    a.TestId == id
                );

        if (hasAttempts)
        {
            return BadRequest(
                "Test already has user attempts and can no longer be edited."
            );
        }


        if (test.IsPublished)
        {
            return BadRequest(
                "Unpublish the test before editing it."
            );
        }


        var technology =
            await _context.Technologies
                .FindAsync(
                    request.TechnologyId
                );

        if (technology == null)
        {
            return BadRequest(
                "Technology not found."
            );
        }


        var questionIds =
            request.QuestionIds
                .Distinct()
                .ToList();

        if (questionIds.Count == 0)
        {
            return BadRequest(
                "Test must contain at least one question."
            );
        }


        var questions =
            await _context.Questions
                .Where(q =>
                    questionIds.Contains(q.Id)
                )
                .Select(q => new
                {
                    q.Id,

                    TechnologyId =
                        q.Topic.TechnologyId
                })
                .ToListAsync();

        if (questions.Count != questionIds.Count)
        {
            return BadRequest(
                "One or more questions do not exist."
            );
        }


        if (questions.Any(q =>
            q.TechnologyId !=
            request.TechnologyId))
        {
            return BadRequest(
                "All questions must belong to the selected technology."
            );
        }


        test.Title =
            request.Title.Trim();

        test.Description =
            request.Description;

        test.TechnologyId =
            request.TechnologyId;


        _context.TestQuestions
            .RemoveRange(
                test.TestQuestions
            );

        test.TestQuestions.Clear();


        for (
            var i = 0;
            i < questionIds.Count;
            i++
        )
        {
            test.TestQuestions.Add(
                new TestQuestion
                {
                    TestId =
                        test.Id,

                    QuestionId =
                        questionIds[i],

                    OrderNumber =
                        i + 1
                }
            );
        }


        await _context.SaveChangesAsync();

        return Ok(new
        {
            message =
                "Test updated successfully."
        });
    }


    [HttpPatch("{id}/publish")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Publish(
        int id)
    {
        var test =
            await _context.Tests
                .Include(t =>
                    t.TestQuestions
                )
                .Include(t =>
                    t.Technology
                )
                .FirstOrDefaultAsync(t =>
                    t.Id == id
                );

        if (test == null)
        {
            return NotFound(
                "Test not found."
            );
        }


        if (test.TestQuestions.Count == 0)
        {
            return BadRequest(
                "Test cannot be published without questions."
            );
        }


        test.IsPublished = true;

        await _context.SaveChangesAsync();


        await _notificationHub
            .Clients
            .All
            .SendAsync(
                "TestPublished",
                new
                {
                    test.Id,
                    test.Title,
                    test.TechnologyId,

                    TechnologyName =
                        test.Technology.Name
                }
            );


        return Ok(new
        {
            message =
                "Test published successfully."
        });
    }


    [HttpPatch("{id}/unpublish")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Unpublish(
        int id)
    {
        var test =
            await _context.Tests
                .FindAsync(id);

        if (test == null)
        {
            return NotFound(
                "Test not found."
            );
        }


        test.IsPublished = false;

        await _context.SaveChangesAsync();


        await _notificationHub
            .Clients
            .All
            .SendAsync(
                "TestUnpublished",
                new
                {
                    test.Id
                }
            );


        return Ok(new
        {
            message =
                "Test unpublished successfully."
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(
        int id)
    {
        var test =
            await _context.Tests
                .FirstOrDefaultAsync(t =>
                    t.Id == id
                );

        if (test == null)
        {
            return NotFound(
                "Test not found."
            );
        }


        if (test.IsPublished)
        {
            return BadRequest(
                "Unpublish the test before deleting it."
            );
        }


        var hasAttempts =
            await _context.TestAttempts
                .AnyAsync(a =>
                    a.TestId == id
                );

        if (hasAttempts)
        {
            return BadRequest(
                "Test has user attempts and cannot be deleted."
            );
        }


        _context.Tests.Remove(test);

        await _context.SaveChangesAsync();


        return Ok(new
        {
            message =
                "Test deleted successfully."
        });
    }
}