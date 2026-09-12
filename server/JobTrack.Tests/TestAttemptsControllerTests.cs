using System.Security.Claims;

using JobTrack.Api.Controllers;
using JobTrack.Api.Data;
using JobTrack.Api.DTOs.TestAttempts;
using JobTrack.Api.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobTrack.Tests;

public class TestAttemptsControllerTests
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


    private static TestAttemptsController CreateController(
        AppDbContext context,
        int userId)
    {
        var controller =
            new TestAttemptsController(context);

        var user =
            new ClaimsPrincipal(
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


    private static async Task SeedTestData(
        AppDbContext context,
        bool isPublished)
    {
        var technology =
            new Technology
            {
                Id = 100,
                Name = "Test Technology"
            };


        var topic =
            new Topic
            {
                Id = 100,
                Name = "Test Topic",
                Content = "Test theory content.",
                TechnologyId = 100
            };


        var question =
            new Question
            {
                Id = 100,
                Text = "What is the correct answer?",
                TopicId = 100
            };


        var correctAnswer =
            new AnswerOption
            {
                Id = 100,
                Text = "Correct",
                IsCorrect = true,
                QuestionId = 100
            };


        var wrongAnswer =
            new AnswerOption
            {
                Id = 101,
                Text = "Wrong",
                IsCorrect = false,
                QuestionId = 100
            };


        var test =
            new Test
            {
                Id = 100,
                Title = "Test Exam",
                Description =
                    "Test description",

                TechnologyId = 100,

                IsPublished =
                    isPublished
            };


        var testQuestion =
            new TestQuestion
            {
                TestId = 100,
                QuestionId = 100,
                OrderNumber = 1
            };


        context.Technologies.Add(
            technology
        );

        context.Topics.Add(
            topic
        );

        context.Questions.Add(
            question
        );

        context.AnswerOptions.AddRange(
            correctAnswer,
            wrongAnswer
        );

        context.Tests.Add(
            test
        );

        context.TestQuestions.Add(
            testQuestion
        );


        await context.SaveChangesAsync();
    }


    [Fact]
    public async Task StartTest_WhenTestIsPublished_CreatesAttempt()
    {
        await using var context =
            CreateContext();


        await SeedTestData(
            context,
            true
        );


        var controller =
            CreateController(
                context,
                1
            );


        var result =
            await controller.StartTest(
                100
            );


        Assert.IsType<
            OkObjectResult
        >(result);


        var attempt =
            await context.TestAttempts
                .SingleAsync();


        Assert.Equal(
            1,
            attempt.UserId
        );

        Assert.Equal(
            100,
            attempt.TestId
        );

        Assert.Equal(
            1,
            attempt.TotalQuestions
        );

        Assert.Null(
            attempt.CompletedAt
        );
    }


    [Fact]
    public async Task StartTest_WhenTestIsNotPublished_ReturnsNotFound()
    {
        await using var context =
            CreateContext();


        await SeedTestData(
            context,
            false
        );


        var controller =
            CreateController(
                context,
                1
            );


        var result =
            await controller.StartTest(
                100
            );


        Assert.IsType<
            NotFoundObjectResult
        >(result);


        Assert.Empty(
            context.TestAttempts
        );
    }


    [Fact]
    public async Task SubmitTest_WhenAttemptAlreadyCompleted_ReturnsBadRequest()
    {
        await using var context =
            CreateContext();


        await SeedTestData(
            context,
            true
        );


        var attempt =
            new TestAttempt
            {
                Id = 100,
                UserId = 1,
                TestId = 100,

                StartedAt =
                    DateTime.UtcNow
                        .AddMinutes(-5),

                CompletedAt =
                    DateTime.UtcNow,

                CorrectAnswers = 1,
                TotalQuestions = 1
            };


        context.TestAttempts.Add(
            attempt
        );

        await context.SaveChangesAsync();


        var controller =
            CreateController(
                context,
                1
            );


        var request =
            new SubmitTestRequest();


        var result =
            await controller.SubmitTest(
                100,
                request
            );


        var badRequest =
            Assert.IsType<
                BadRequestObjectResult
            >(result);


        Assert.Equal(
            "This test attempt has already been submitted.",
            badRequest.Value
        );
    }
}