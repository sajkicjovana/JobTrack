using JobTrack.Api.Data;
using JobTrack.Api.DTOs.Topics;
using JobTrack.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TopicsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TopicsController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll(
        int? technologyId)
    {
        var query = _context.Topics.AsQueryable();

        if (technologyId.HasValue)
        {
            query = query.Where(t =>
                t.TechnologyId == technologyId.Value
            );
        }

        var topics = await query
            .OrderBy(t => t.Technology.Name)
            .ThenBy(t => t.Name)
            .Select(t => new
            {
                t.Id,
                t.Name,
                t.Content,
                t.TechnologyId,

                TechnologyName =
                    t.Technology.Name,

                QuestionCount =
                    t.Questions.Count
            })
            .ToListAsync();

        return Ok(topics);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var topic = await _context.Topics
            .Where(t => t.Id == id)
            .Select(t => new
            {
                t.Id,
                t.Name,
                t.Content,
                t.TechnologyId,

                TechnologyName =
                    t.Technology.Name,

                QuestionCount =
                    t.Questions.Count
            })
            .FirstOrDefaultAsync();

        if (topic == null)
        {
            return NotFound(
                "Topic not found."
            );
        }

        return Ok(topic);
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        TopicRequest request)
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

        var name = request.Name.Trim();

        var exists = await _context.Topics
            .AnyAsync(t =>
                t.TechnologyId == request.TechnologyId &&
                t.Name.ToLower() == name.ToLower()
            );

        if (exists)
        {
            return BadRequest(
                "Topic already exists for this technology."
            );
        }

        var topic = new Topic
        {
            Name = name,
            Content = request.Content.Trim(),
            TechnologyId = request.TechnologyId
        };

        _context.Topics.Add(topic);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Topic created successfully.",
            id = topic.Id
        });
    }


    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int id,
        TopicRequest request)
    {
        var topic = await _context.Topics
            .FindAsync(id);

        if (topic == null)
        {
            return NotFound(
                "Topic not found."
            );
        }

        var technology =
            await _context.Technologies
                .FindAsync(request.TechnologyId);

        if (technology == null)
        {
            return BadRequest(
                "Technology not found."
            );
        }

        var name = request.Name.Trim();

        var exists = await _context.Topics
            .AnyAsync(t =>
                t.Id != id &&
                t.TechnologyId == request.TechnologyId &&
                t.Name.ToLower() == name.ToLower()
            );

        if (exists)
        {
            return BadRequest(
                "Topic already exists for this technology."
            );
        }

        topic.Name = name;
        topic.Content = request.Content.Trim();
        topic.TechnologyId = request.TechnologyId;
        
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Topic updated successfully."
        });
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var topic = await _context.Topics
            .FindAsync(id);

        if (topic == null)
        {
            return NotFound(
                "Topic not found."
            );
        }

        var hasQuestions =
            await _context.Questions
                .AnyAsync(q =>
                    q.TopicId == id
                );

        if (hasQuestions)
        {
            return BadRequest(
                "Topic contains questions and cannot be deleted."
            );
        }

        _context.Topics.Remove(topic);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Topic deleted successfully."
        });
    }
}