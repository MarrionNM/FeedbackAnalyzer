namespace FeedbackAnalyzer.Data.Payloads;

public class Filter
{
    public string? Search { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public string Sentiment { get; set; } = "";
    public string Tag { get; set; } = "";
}
