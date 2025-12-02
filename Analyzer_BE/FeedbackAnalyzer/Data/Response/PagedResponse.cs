namespace FeedbackAnalyzer.Data.Response;

public class PagedResponse<T>
{
    public List<T> Data { get; set; } = [];

    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int RecordsOnPage => Data?.Count ?? 0;
}
