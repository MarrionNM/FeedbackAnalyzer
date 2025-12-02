namespace FeedbackAnalyzer.Data.DTO;

public class FeedbackTagDTO : BaseDTO
{
    public required string FeedbackId { get; set; }
    public required string TagId { get; set; }
    public TagDTO Tag { get; set; } = default!;
}
