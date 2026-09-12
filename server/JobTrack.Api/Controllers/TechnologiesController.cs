using JobTrack.Api.Data;
using JobTrack.Api.DTOs.Technologies;
using JobTrack.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TechnologiesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TechnologiesController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var technologies = await _context.Technologies
            .OrderBy(t => t.Name)
            .Select(t => new
            {
                t.Id,
                t.Name
            })
            .ToListAsync();

        return Ok(technologies);
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        TechnologyRequest request)
    {
        var name = request.Name.Trim();

        var exists = await _context.Technologies
            .AnyAsync(t =>
                t.Name.ToLower() == name.ToLower()
            );

        if (exists)
        {
            return BadRequest(
                "Technology already exists."
            );
        }

        var technology = new Technology
        {
            Name = name
        };

        _context.Technologies.Add(technology);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Technology created successfully.",
            id = technology.Id
        });
    }


    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int id,
        TechnologyRequest request)
    {
        var technology = await _context.Technologies
            .FindAsync(id);

        if (technology == null)
        {
            return NotFound(
                "Technology not found."
            );
        }

        var name = request.Name.Trim();

        var exists = await _context.Technologies
            .AnyAsync(t =>
                t.Id != id &&
                t.Name.ToLower() == name.ToLower()
            );

        if (exists)
        {
            return BadRequest(
                "Technology already exists."
            );
        }

        technology.Name = name;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Technology updated successfully."
        });
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var technology = await _context.Technologies
            .FindAsync(id);

        if (technology == null)
        {
            return NotFound(
                "Technology not found."
            );
        }

        var usedInApplications =
            await _context.JobApplicationTechnologies
                .AnyAsync(jt =>
                    jt.TechnologyId == id
                );

        var hasTopics =
            await _context.Topics
                .AnyAsync(t =>
                    t.TechnologyId == id
                );

        var hasTests =
            await _context.Tests
                .AnyAsync(t =>
                    t.TechnologyId == id
                );

        if (
            usedInApplications ||
            hasTopics ||
            hasTests
        )
        {
            return BadRequest(
                "Technology is currently in use and cannot be deleted."
            );
        }

        _context.Technologies.Remove(technology);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Technology deleted successfully."
        });
    }
}