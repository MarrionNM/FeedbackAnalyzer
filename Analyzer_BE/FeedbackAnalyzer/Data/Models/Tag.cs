namespace FeedbackAnalyzer.Data.Models;

public class Tag : Base
{
    public string Name { get; set; } = string.Empty;
    public List<FeedbackTag> FeedbackTags { get; set; } = [];
}
