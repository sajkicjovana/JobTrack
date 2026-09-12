using JobTrack.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTrack.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<JobApplication> JobApplications { get; set; }

    public DbSet<Technology> Technologies { get; set; }

    public DbSet<JobApplicationTechnology> JobApplicationTechnologies { get; set; }

    public DbSet<Interview> Interviews { get; set; }

    public DbSet<Topic> Topics { get; set; }

    public DbSet<Question> Questions { get; set; }

    public DbSet<AnswerOption> AnswerOptions { get; set; }

    public DbSet<Test> Tests { get; set; }

    public DbSet<TestQuestion> TestQuestions { get; set; }

    public DbSet<TestAttempt> TestAttempts { get; set; }

    public DbSet<TestAnswer> TestAnswers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // JOB APPLICATION STATUS

        modelBuilder.Entity<JobApplication>()
            .Property(j => j.Status)
            .HasConversion<string>();


        // INTERVIEWS

        modelBuilder.Entity<Interview>()
            .Property(i => i.Type)
            .HasConversion<string>();

        modelBuilder.Entity<Interview>()
            .Property(i => i.Outcome)
            .HasConversion<string>();


        // JOB APPLICATION - TECHNOLOGY

        modelBuilder.Entity<JobApplicationTechnology>()
            .HasKey(jt => new
            {
                jt.JobApplicationId,
                jt.TechnologyId
            });

        modelBuilder.Entity<JobApplicationTechnology>()
            .HasOne(jt => jt.JobApplication)
            .WithMany(j => j.JobApplicationTechnologies)
            .HasForeignKey(jt => jt.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobApplicationTechnology>()
            .HasOne(jt => jt.Technology)
            .WithMany(t => t.JobApplicationTechnologies)
            .HasForeignKey(jt => jt.TechnologyId)
            .OnDelete(DeleteBehavior.Restrict);


        // TECHNOLOGY

        modelBuilder.Entity<Technology>()
            .HasIndex(t => t.Name)
            .IsUnique();


        // TOPIC

        modelBuilder.Entity<Topic>()
            .HasIndex(t => new
            {
                t.TechnologyId,
                t.Name
            })
            .IsUnique();

        modelBuilder.Entity<Topic>()
            .HasOne(t => t.Technology)
            .WithMany(t => t.Topics)
            .HasForeignKey(t => t.TechnologyId)
            .OnDelete(DeleteBehavior.Restrict);


        // QUESTION - ANSWERS

        modelBuilder.Entity<AnswerOption>()
            .HasOne(a => a.Question)
            .WithMany(q => q.AnswerOptions)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);


        // TEST - TECHNOLOGY

        modelBuilder.Entity<Test>()
            .HasOne(t => t.Technology)
            .WithMany(t => t.Tests)
            .HasForeignKey(t => t.TechnologyId)
            .OnDelete(DeleteBehavior.Restrict);


        // TEST - QUESTION

        modelBuilder.Entity<TestQuestion>()
            .HasKey(tq => new
            {
                tq.TestId,
                tq.QuestionId
            });

        modelBuilder.Entity<TestQuestion>()
            .HasOne(tq => tq.Test)
            .WithMany(t => t.TestQuestions)
            .HasForeignKey(tq => tq.TestId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TestQuestion>()
            .HasOne(tq => tq.Question)
            .WithMany(q => q.TestQuestions)
            .HasForeignKey(tq => tq.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);


        // DEFAULT TECHNOLOGIES

        modelBuilder.Entity<Technology>().HasData(
            new Technology { Id = 1, Name = "C#" },
            new Technology { Id = 2, Name = ".NET" },
            new Technology { Id = 3, Name = "ASP.NET Core" },
            new Technology { Id = 4, Name = "Angular" },
            new Technology { Id = 5, Name = "React" },
            new Technology { Id = 6, Name = "JavaScript" },
            new Technology { Id = 7, Name = "TypeScript" },
            new Technology { Id = 8, Name = "Java" },
            new Technology { Id = 9, Name = "Python" },
            new Technology { Id = 10, Name = "SQL" },
            new Technology { Id = 11, Name = "PostgreSQL" },
            new Technology { Id = 12, Name = "Node.js" },
            new Technology { Id = 13, Name = "Git" },
            new Technology { Id = 14, Name = "Docker" }
        );

        modelBuilder.Entity<TestAttempt>()
            .HasOne(a => a.User)
            .WithMany(u => u.TestAttempts)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TestAttempt>()
            .HasOne(a => a.Test)
            .WithMany(t => t.TestAttempts)
            .HasForeignKey(a => a.TestId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TestAnswer>()
            .HasOne(a => a.TestAttempt)
            .WithMany(a => a.Answers)
            .HasForeignKey(a => a.TestAttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TestAnswer>()
            .HasOne(a => a.Question)
            .WithMany(q => q.TestAnswers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TestAnswer>()
            .HasOne(a => a.SelectedAnswerOption)
            .WithMany()
            .HasForeignKey(a => a.SelectedAnswerOptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}