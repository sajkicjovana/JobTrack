using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using JobTrack.Api.Models;

namespace JobTrack.Api.Data;

public static class PreparationSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        string contentRootPath)
    {

        if (await context.Tests.AnyAsync() ||
            await context.Questions.AnyAsync())
        {
            return;
        }

        var filePath = Path.Combine(
            contentRootPath,
            "Data",
            "Seed",
            "jobtrack_test_dataset.json"
        );

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                "Preparation seed file was not found.",
                filePath
            );
        }

        var json =
            await File.ReadAllTextAsync(filePath);

        var seedData =
            JsonSerializer.Deserialize<PreparationSeedData>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

        if (seedData == null)
        {
            throw new Exception(
                "Preparation seed data could not be loaded."
            );
        }


        var technologies =
            await context.Technologies.ToListAsync();

        var technologyNames =
            seedData.Topics
                .Select(t => t.TechnologyName)
                .Distinct(
                    StringComparer.OrdinalIgnoreCase
                )
                .ToList();


        foreach (var technologyName in technologyNames)
        {
            var exists =
                technologies.Any(t =>
                    t.Name.Equals(
                        technologyName,
                        StringComparison.OrdinalIgnoreCase
                    )
                );

            if (!exists)
            {
                var technology =
                    new Technology
                    {
                        Name = technologyName
                    };

                context.Technologies.Add(technology);
                technologies.Add(technology);
            }
        }

        await context.SaveChangesAsync();


        var topicDictionary =
            new Dictionary<string, Topic>();


        foreach (var seedTopic in seedData.Topics)
        {
            var technology =
                technologies.First(t =>
                    t.Name.Equals(
                        seedTopic.TechnologyName,
                        StringComparison.OrdinalIgnoreCase
                    )
                );

            var topic = new Topic
            {
                Name = seedTopic.Name,
                Content = seedTopic.Content,
                TechnologyId = technology.Id
            };


            context.Topics.Add(topic);

            topicDictionary[
                CreateTopicKey(
                    seedTopic.TechnologyName,
                    seedTopic.Name
                )
            ] = topic;
        }


        await context.SaveChangesAsync();


        var questionDictionary =
            new Dictionary<string, Question>();


        foreach (var seedQuestion in seedData.Questions)
        {
            var topicKey =
                CreateTopicKey(
                    seedQuestion.TechnologyName,
                    seedQuestion.TopicName
                );


            var topic =
                topicDictionary[topicKey];


            var question = new Question
            {
                Text = seedQuestion.Text,
                TopicId = topic.Id
            };


            context.Questions.Add(question);

            await context.SaveChangesAsync();


            foreach (var seedAnswer
                     in seedQuestion.Answers)
            {
                var answer =
                    new AnswerOption
                    {
                        Text = seedAnswer.Text,
                        IsCorrect =
                            seedAnswer.IsCorrect,

                        QuestionId =
                            question.Id
                    };


                context.AnswerOptions.Add(answer);
            }


            questionDictionary[
                seedQuestion.Code
            ] = question;


            await context.SaveChangesAsync();
        }



        foreach (var seedTest in seedData.Tests)
        {
            var technology =
                technologies.First(t =>
                    t.Name.Equals(
                        seedTest.TechnologyName,
                        StringComparison.OrdinalIgnoreCase
                    )
                );


            var test = new Test
            {
                Title = seedTest.Title,
                Description =
                    seedTest.Description,

                IsPublished =
                    seedTest.IsPublished,

                CreatedAt =
                    DateTime.UtcNow,

                TechnologyId =
                    technology.Id
            };


            context.Tests.Add(test);

            await context.SaveChangesAsync();


            for (
                var i = 0;
                i < seedTest.QuestionCodes.Count;
                i++
            )
            {
                var code =
                    seedTest.QuestionCodes[i];


                if (!questionDictionary
                        .TryGetValue(
                            code,
                            out var question
                        ))
                {
                    throw new Exception(
                        $"Question code '{code}' was not found."
                    );
                }


                context.TestQuestions.Add(
                    new TestQuestion
                    {
                        TestId = test.Id,
                        QuestionId =
                            question.Id,

                        OrderNumber =
                            i + 1
                    }
                );
            }


            await context.SaveChangesAsync();
        }
    }


    private static string CreateTopicKey(
        string technology,
        string topic)
    {
        return
            $"{technology.Trim().ToLowerInvariant()}|" +
            $"{topic.Trim().ToLowerInvariant()}";
    }
}




public class PreparationSeedData
{
    public List<SeedTopic> Topics { get; set; } = [];
    public List<SeedQuestion> Questions { get; set; } = [];
    public List<SeedTest> Tests { get; set; } = [];
}


public class SeedTopic
{
    public string TechnologyName { get; set; } = "";
    public string Name { get; set; } = "";
    public string Content { get; set; } = "";
}

public class SeedQuestion
{
    public string Code { get; set; } = "";

    public string TechnologyName { get; set; } = "";

    public string TopicName { get; set; } = "";

    public string Text { get; set; } = "";

    public List<SeedAnswer> Answers { get; set; } = [];
}


public class SeedAnswer
{
    public string Text { get; set; } = "";

    public bool IsCorrect { get; set; }
}


public class SeedTest
{
    public string TechnologyName { get; set; } = "";

    public string Title { get; set; } = "";

    public string? Description { get; set; }

    public bool IsPublished { get; set; }

    public List<string> QuestionCodes { get; set; } = [];
}