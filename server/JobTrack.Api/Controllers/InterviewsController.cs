using System.Security.Claims;
using JobTrack.Api.Data;
using JobTrack.Api.DTOs.Interviews;
using JobTrack.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InterviewsController : ControllerBase
{
    private readonly AppDbContext _context;

    public InterviewsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim);

        var interviews = await _context.Interviews
            .Where(i =>
                i.JobApplication.UserId == userId
            )
            .OrderBy(i => i.InterviewDate)
            .Select(i => new
            {
                i.Id,
                i.JobApplicationId,
                i.InterviewDate,
                i.Type,
                i.ContactPerson,
                i.Notes,
                i.Outcome,

                CompanyName =
                    i.JobApplication.CompanyName,

                Position =
                    i.JobApplication.Position
            })
            .ToListAsync();

        return Ok(interviews);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim);

        var interview = await _context.Interviews
            .Where(i =>
                i.Id == id &&
                i.JobApplication.UserId == userId
            )
            .Select(i => new
            {
                i.Id,
                i.JobApplicationId,
                i.InterviewDate,
                i.Type,
                i.ContactPerson,
                i.Notes,
                i.Outcome,

                CompanyName =
                    i.JobApplication.CompanyName,

                Position =
                    i.JobApplication.Position
            })
            .FirstOrDefaultAsync();

        if (interview == null)
        {
            return NotFound("Interview not found.");
        }

        return Ok(interview);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateInterviewRequest request)
    {
        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim);

        var jobApplication =
            await _context.JobApplications
                .FirstOrDefaultAsync(j =>
                    j.Id == request.JobApplicationId &&
                    j.UserId == userId
                );

        if (jobApplication == null)
        {
            return BadRequest(
                "Job application not found."
            );
        }

        var interview = new Interview
        {
            JobApplicationId =
                request.JobApplicationId,

            InterviewDate =
                request.InterviewDate,

            Type =
                request.Type,

            ContactPerson =
                request.ContactPerson,

            Notes =
                request.Notes,

            Outcome =
                request.Outcome
        };

        _context.Interviews.Add(interview);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message =
                "Interview created successfully.",

            id = interview.Id
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateInterviewRequest request)
    {
        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim);

        var interview =
            await _context.Interviews
                .Include(i => i.JobApplication)
                .FirstOrDefaultAsync(i =>
                    i.Id == id &&
                    i.JobApplication.UserId == userId
                );

        if (interview == null)
        {
            return NotFound(
                "Interview not found."
            );
        }

        var jobApplication =
            await _context.JobApplications
                .FirstOrDefaultAsync(j =>
                    j.Id == request.JobApplicationId &&
                    j.UserId == userId
                );

        if (jobApplication == null)
        {
            return BadRequest(
                "Job application not found."
            );
        }

        interview.JobApplicationId =
            request.JobApplicationId;

        interview.InterviewDate =
            request.InterviewDate;

        interview.Type =
            request.Type;

        interview.ContactPerson =
            request.ContactPerson;

        interview.Notes =
            request.Notes;

        interview.Outcome =
            request.Outcome;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message =
                "Interview updated successfully."
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim);

        var interview =
            await _context.Interviews
                .Include(i => i.JobApplication)
                .FirstOrDefaultAsync(i =>
                    i.Id == id &&
                    i.JobApplication.UserId == userId
                );

        if (interview == null)
        {
            return NotFound(
                "Interview not found."
            );
        }

        _context.Interviews.Remove(interview);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message =
                "Interview deleted successfully."
        });
    }
}