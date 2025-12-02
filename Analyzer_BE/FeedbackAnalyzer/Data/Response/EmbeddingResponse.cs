namespace FeedbackAnalyzer.Data.Response;

public class EmbeddingResponse
{
    public List<EmbeddingData> Data { get; set; } = [];
}

public class EmbeddingData
{
    public List<float> Embedding { get; set; } = [];
}