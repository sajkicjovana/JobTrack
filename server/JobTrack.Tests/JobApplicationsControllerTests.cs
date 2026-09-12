using System.Security.Claims;

using JobTrack.Api.Controllers;
using JobTrack.Api.Data;
using JobTrack.Api.DTOs.JobApplications;
using JobTrack.Api.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobTrack.Tests;

public class JobApplicationsControllerTests
{
    private static AppDbContext CreateContext()
    {
        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString()
                )
                .Options;

        return new AppDbContext(options);
    }


    private static JobApplicationsController CreateController(
        AppDbContext context,
        int userId)
    {
        var controller =
            new JobApplicationsController(context);

        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        userId.ToString()
                    )
                },
                "TestAuthentication"
            )
        );

        controller.ControllerContext =
            new ControllerContext
            {
                HttpContext =
                    new DefaultHttpContext
                    {
                        User = user
                    }
            };

        return controller;
    }


    [Fact]
    public async Task Create_WithValidTechnology_SavesApplication()
    {
        await using var context =
            CreateContext();

        context.Technologies.Add(
            new Technology
            {
                Id = 100,
                Name = "Test Technology"
            }
        );

        await context.SaveChangesAsync();


        var controller =
            CreateController(
                context,
                1
            );


        var request =
            new CreateJobApplicationRequest
            {
                CompanyName =
                    "Test Company",

                Position =
                    "Junior Developer",

                Status =
                    JobApplicationStatus.Applied,

                ApplicationDate =
                    DateTime.UtcNow,

                TechnologyIds =
                    new List<int>
                    {
                        100
                    }
            };


        var result =
            await controller.Create(
                request
            );


        Assert.IsType<OkObjectResult>(
            result
        );


        var application =
            await context.JobApplications
                .Include(a =>
                    a.JobApplicationTechnologies
                )
                .SingleAsync();


        Assert.Equal(
            "Test Company",
            application.CompanyName
        );

        Assert.Equal(
            "Junior Developer",
            application.Position
        );

        Assert.Equal(
            1,
            application.UserId
        );

        Assert.Single(
            application
                .JobApplicationTechnologies
        );

        Assert.Equal(
            100,
            application
                .JobApplicationTechnologies
                .Single()
                .TechnologyId
        );
    }


    [Fact]
    public async Task Create_WithInvalidTechnology_ReturnsBadRequest()
    {
        await using var context =
            CreateContext();


        var controller =
            CreateController(
                context,
                1
            );


        var request =
            new CreateJobApplicationRequest
            {
                CompanyName =
                    "Test Company",

                Position =
                    "Developer",

                Status =
                    JobApplicationStatus.Saved,

                ApplicationDate =
                    DateTime.UtcNow,

                TechnologyIds =
                    new List<int>
                    {
                        999
                    }
            };


        var result =
            await controller.Create(
                request
            );


        Assert.IsType<
            BadRequestObjectResult
        >(result);


        Assert.Empty(
            context.JobApplications
        );
    }


    [Fact]
    public async Task GetById_ForAnotherUsersApplication_ReturnsNotFound()
    {
        await using var context =
            CreateContext();


        var application =
            new JobApplication
            {
                CompanyName =
                    "Other Company",

                Position =
                    "Developer",

                Status =
                    JobApplicationStatus.Applied,

                ApplicationDate =
                    DateTime.UtcNow,

                UserId = 2
            };


        context.JobApplications.Add(
            application
        );

        await context.SaveChangesAsync();


        var controller =
            CreateController(
                context,
                1
            );


        var result =
            await controller.GetById(
                application.Id
            );


        Assert.IsType<
            NotFoundObjectResult
        >(result);
    }


    [Fact]
    public async Task Delete_ForAnotherUsersApplication_ReturnsNotFound()
    {
        await using var context =
            CreateContext();


        var application =
            new JobApplication
            {
                CompanyName =
                    "Other Company",

                Position =
                    "Developer",

                Status =
                    JobApplicationStatus.Applied,

                ApplicationDate =
                    DateTime.UtcNow,

                UserId = 2
            };


        context.JobApplications.Add(
            application
        );

        await context.SaveChangesAsync();


        var controller =
            CreateController(
                context,
                1
            );


        var result =
            await controller.Delete(
                application.Id
            );


        Assert.IsType<
            NotFoundObjectResult
        >(result);


        Assert.Single(
            context.JobApplications
        );
    }
}