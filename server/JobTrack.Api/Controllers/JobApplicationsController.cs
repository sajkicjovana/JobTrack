using System.Security.Claims;
using JobTrack.Api.Data;
using JobTrack.Api.DTOs.JobApplications;
using JobTrack.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobApplicationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public JobApplicationsController(AppDbContext context)
    {
        _context = context;
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        CreateJobApplicationRequest request)
    {
        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim);

        var technologyIds = request.TechnologyIds
            .Distinct()
            .ToList();


        if (technologyIds.Count > 0)
        {
            var existingTechnologyIds =
                await _context.Technologies
                    .Where(t => technologyIds.Contains(t.Id))
                    .Select(t => t.Id)
                    .ToListAsync();

            if (existingTechnologyIds.Count != technologyIds.Count)
            {
                return BadRequest(
                    "One or more technologies are invalid."
                );
            }
        }


        var jobApplication = new JobApplication
        {
            CompanyName = request.CompanyName,
            Position = request.Position,
            JobUrl = request.JobUrl,
            Location = request.Location,
            Notes = request.Notes,
            Status = request.Status,
            ApplicationDate = request.ApplicationDate,
            UserId = userId
        };


        foreach (var technologyId in technologyIds)
        {
            jobApplication.JobApplicationTechnologies.Add(
                new JobApplicationTechnology
                {
                    TechnologyId = technologyId
                }
            );
        }


        _context.JobApplications.Add(jobApplication);

        await _context.SaveChangesAsync();


        return Ok(new
        {
            message = "Job application created successfully.",
            id = jobApplication.Id
        });
    }


    [HttpGet]
    public async Task<IActionResult> GetAll(
        JobApplicationStatus? status,
        string? search,
        int? technologyId,
        DateTime? fromDate,
        DateTime? toDate)
    {
        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim);


        var query = _context.JobApplications
            .Where(j => j.UserId == userId);


        if (status.HasValue)
        {
            query = query.Where(
                j => j.Status == status.Value
            );
        }


        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchValue = search.Trim();

            query = query.Where(j =>
                EF.Functions.ILike(
                    j.CompanyName,
                    $"%{searchValue}%"
                ) ||
                EF.Functions.ILike(
                    j.Position,
                    $"%{searchValue}%"
                )
            );
        }


        if (technologyId.HasValue)
        {
            query = query.Where(j =>
                j.JobApplicationTechnologies
                    .Any(jt =>
                        jt.TechnologyId ==
                        technologyId.Value
                    )
            );
        }


        if (fromDate.HasValue)
        {
            query = query.Where(
                j => j.ApplicationDate >= fromDate.Value
            );
        }


        if (toDate.HasValue)
        {
            query = query.Where(
                j => j.ApplicationDate <= toDate.Value
            );
        }


        var jobApplications = await query
            .OrderByDescending(j =>
                j.ApplicationDate
            )
            .Select(j => new
            {
                j.Id,
                j.CompanyName,
                j.Position,
                j.JobUrl,
                j.Location,
                j.Notes,
                j.Status,
                j.ApplicationDate,

                Technologies =
                    j.JobApplicationTechnologies
                        .Select(jt => new
                        {
                            jt.Technology.Id,
                            jt.Technology.Name
                        })
                        .OrderBy(t => t.Name)
                        .ToList()
            })
            .ToListAsync();


        return Ok(jobApplications);
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


        var jobApplication = await _context.JobApplications
            .Where(j =>
                j.Id == id &&
                j.UserId == userId
            )
            .Select(j => new
            {
                j.Id,
                j.CompanyName,
                j.Position,
                j.JobUrl,
                j.Location,
                j.Notes,
                j.Status,
                j.ApplicationDate,

                Technologies = j.JobApplicationTechnologies
                    .Select(jt => new
                    {
                        jt.Technology.Id,
                        jt.Technology.Name
                    })
                    .OrderBy(t => t.Name)
                    .ToList()
            })
            .FirstOrDefaultAsync();


        if (jobApplication == null)
        {
            return NotFound(
                "Job application not found."
            );
        }


        return Ok(jobApplication);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateJobApplicationRequest request)
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
                .Include(j =>
                    j.JobApplicationTechnologies
                )
                .FirstOrDefaultAsync(j =>
                    j.Id == id &&
                    j.UserId == userId
                );


        if (jobApplication == null)
        {
            return NotFound(
                "Job application not found."
            );
        }


        var technologyIds = request.TechnologyIds
            .Distinct()
            .ToList();


        if (technologyIds.Count > 0)
        {
            var existingTechnologyIds =
                await _context.Technologies
                    .Where(t =>
                        technologyIds.Contains(t.Id)
                    )
                    .Select(t => t.Id)
                    .ToListAsync();

            if (existingTechnologyIds.Count != technologyIds.Count)
            {
                return BadRequest(
                    "One or more technologies are invalid."
                );
            }
        }


        jobApplication.CompanyName =
            request.CompanyName;

        jobApplication.Position =
            request.Position;

        jobApplication.JobUrl =
            request.JobUrl;

        jobApplication.Location =
            request.Location;

        jobApplication.Notes =
            request.Notes;

        jobApplication.Status =
            request.Status;

        jobApplication.ApplicationDate =
            request.ApplicationDate;


        var currentTechnologyIds =
            jobApplication.JobApplicationTechnologies
                .Select(jt => jt.TechnologyId)
                .ToList();


        var technologiesToRemove =
            jobApplication.JobApplicationTechnologies
                .Where(jt =>
                    !technologyIds.Contains(
                        jt.TechnologyId
                    )
                )
                .ToList();


        _context.JobApplicationTechnologies
            .RemoveRange(technologiesToRemove);


        var technologiesToAdd =
            technologyIds.Except(
                currentTechnologyIds
            );


        foreach (var technologyId in technologiesToAdd)
        {
            jobApplication.JobApplicationTechnologies.Add(
                new JobApplicationTechnology
                {
                    JobApplicationId = jobApplication.Id,
                    TechnologyId = technologyId
                }
            );
        }


        await _context.SaveChangesAsync();


        return Ok(new
        {
            message =
                "Job application updated successfully."
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


        var jobApplication =
            await _context.JobApplications
                .FirstOrDefaultAsync(j =>
                    j.Id == id &&
                    j.UserId == userId
                );


        if (jobApplication == null)
        {
            return NotFound(
                "Job application not found."
            );
        }


        _context.JobApplications.Remove(
            jobApplication
        );

        await _context.SaveChangesAsync();


        return Ok(new
        {
            message =
                "Job application deleted successfully."
        });
    }
}