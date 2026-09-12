namespace JobTrack.Api.Models;

public class Topic
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int TechnologyId { get; set; }

    public Technology Technology { get; set; } = null!;

    public ICollection<Question> Questions { get; set; }
        = new List<Question>();
    
    public string Content { get; set; } = "";
}