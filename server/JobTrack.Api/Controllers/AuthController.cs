using JobTrack.Api.Data;
using JobTrack.Api.DTOs.Auth;
using JobTrack.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobTrack.Api.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace JobTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly JwtService _jwtService;


    public AuthController(
        AppDbContext context,
        IPasswordHasher<User> passwordHasher,
        JwtService jwtService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser != null)
        {
            return BadRequest("User with this email already exists.");
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Role = "User"
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            request.Password
        );

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "User registered successfully.",
            userId = user.Id,
            email = user.Email
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

       var passwordResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password
        );

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized("Invalid email or password.");
        }

        var token = _jwtService.GenerateToken(user);

        return Ok(new
        {
            message = "Login successful.",
            token = token,
            userId = user.Id,
            email = user.Email,
            role = user.Role
        });
    
    }

    [Authorize]
[HttpGet("me")]
public IActionResult GetCurrentUser()
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var email = User.FindFirstValue(ClaimTypes.Email);
    var role = User.FindFirstValue(ClaimTypes.Role);

    return Ok(new
    {
        userId,
        email,
        role
    });
}

    

}