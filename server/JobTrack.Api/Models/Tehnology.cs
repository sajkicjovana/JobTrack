namespace JobTrack.Api.Models;

public class Technology
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<JobApplicationTechnology> JobApplicationTechnologies { get; set; }
        = new List<JobApplicationTechnology>();

    public ICollection<Topic> Topics { get; set; }
        = new List<Topic>();

    public ICollection<Test> Tests { get; set; }
        = new List<Test>();
}