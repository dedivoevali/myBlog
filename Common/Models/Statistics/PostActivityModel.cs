namespace Common.Models.Statistics;

public class PostActivityModel()
{
    public PostActivityModel(int postId, TimeMeasure measure) : this()
    {
        Id = postId;
        GeneratedAt = DateTime.UtcNow;
        Measure = measure;
    }
    public int Id { get; set; }
    public DateTime GeneratedAt { get; set; }
    public TimeMeasure Measure { get; set; }
    public IEnumerable<PostActivityStepModel> Steps { get; set; } 
}

public class PostActivityStepModel
{
    public int Reactions { get; set; }
    public int Comments { get; set; }
    public DateOnly StartDate { get; set; }
}