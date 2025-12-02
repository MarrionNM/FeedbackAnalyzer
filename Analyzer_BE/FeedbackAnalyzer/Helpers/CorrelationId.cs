namespace FeedbackAnalyzer.Helpers;

public static class Correlation
{
    private static readonly AsyncLocal<string?> _current = new();

    public static string Current
    {
        get => _current.Value ??= Guid.NewGuid().ToString();
        set => _current.Value = value;
    }

    public static void Reset() => _current.Value = null;
}
