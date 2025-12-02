namespace FeedbackAnalyzer.Data.Response;

public class OpenAIChatResponse
{
    public List<Choice> Choices { get; set; } = [];
}

public class Choice
{
    public ChatMessage Message { get; set; } = default!;
}

public class ChatMessage
{
    public string Content { get; set; } = default!;
}
