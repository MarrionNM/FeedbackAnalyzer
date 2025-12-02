namespace FeedbackAnalyzer.Data.DTO;

public class BaseDTO
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}
