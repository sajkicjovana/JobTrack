namespace JobTrack.Api.Models;

public class JobApplicationTechnology
{
    public int JobApplicationId { get; set; }

    public JobApplication JobApplication { get; set; } = null!;


    public int TechnologyId { get; set; }

    public Technology Technology { get; set; } = null!;
}