namespace FeedbackAnalyzer.Data.Models;

public class Base
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedDate { get; set; } = new();
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DeletedAt { get; set; }
}
