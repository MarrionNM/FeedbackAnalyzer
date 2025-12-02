namespace FeedbackAnalyzer.Data.DTO;

public class TagDTO : BaseDTO
{
    public string Name { get; set; } = string.Empty;
    public List<FeedbackTagDTO> FeedbackTags { get; set; } = [];
}
