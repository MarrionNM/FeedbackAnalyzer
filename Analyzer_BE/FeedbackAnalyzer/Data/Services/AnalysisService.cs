using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FeedbackAnalyzer.Contracts.IServices;
using FeedbackAnalyzer.Data.DTO;
using FeedbackAnalyzer.Data.Response;
using FeedbackAnalyzer.Helpers;

namespace FeedbackAnalyzer.Data.Services;

public class AnalysisService(
    HttpClient http,
    IConfiguration config,
    ILogger<AnalysisService> logger) : IAnalysisService
{
    private readonly ILogger<AnalysisService> _log = logger;
    private readonly string _model = config["OpenAI:Model"]!;
    private readonly string _embeddingModel = config["OpenAI:EmbeddingModel"]!;

    // Simple in-memory cache: stores results by hashed feedback text
    private static readonly Dictionary<string, FeedbackAnalysisDTO> _cache = [];

    private const string SystemPrompt =
        """
            You are an AI that analyzes software application user feedback.  
            You must return ONLY a valid JSON object.  
            No markdown, no code fences, no explanations, no additional text.

            Strict Output Schema:
            {
            "summary": "string (1–50 words, professional, no PII, no quotes)",
            "sentiment": "positive" | "neutral" | "negative",
            "tags": ["tag1", "tag2", ... up to 5 max],
            "priority": "P0" | "P1" | "P2" | "P3",
            "nextAction": "string (1 sentence, specific, no PII)"
            }

            Tag Rules (VERY IMPORTANT):
            - Tags must be common software/system categories.
            - Choose from or generalize to well-known system tags.
            - Prioritize the closest match from typical product-support categories like:
            "billing", "payments", "login", "authentication", "performance",
            "latency", "crash", "bug", "ui", "ux", "navigation", "checkout",
            "notifications", "settings", "account", "security", "support",
            "data", "integration", "sync", "api", "feature-request".
            - Tags must be short nouns (no phrases).
            - Tags must reflect the *core issue*, not wording from the user.
            - Use 1–5 tags max.

            Priority Rules:
            - Use P0 to P3 where:
            - P0 = lowest urgency (minor, cosmetic, non-impacting)
            - P1 = low urgency (inconvenience, workaround exists)
            - P2 = medium urgency (affects functionality or flow)
            - P3 = highest urgency (critical outage, blockers, security risks)

            Safety:
            - The summary must remove PII (names, emails, phones).
            - Keep tone concise, neutral, professional.
            - Never repeat any sensitive personal info.
            - Never include commentary or reasoning.

            If unsure about any field, choose the safest conservative option.
        """;

    public async Task<FeedbackAnalysisDTO> AnalyzeAsync(string feedback)
    {
        var correlationId = Correlation.Current;
        var startTime = DateTime.UtcNow;

        try
        {
            _log.LogInformation("Request started. CorrelationId={CorrelationId}", correlationId);

            string cacheKey = Hash(feedback);

            if (_cache.TryGetValue(cacheKey, out var cached))
            {
                _log.LogInformation(
                    "CACHE HIT. CorrelationId={CorrelationId}, Hash={Hash}",
                    correlationId,
                    cacheKey
                );
                return cached;
            }

            _log.LogInformation(
                "CACHE MISS. CorrelationId={CorrelationId}, Hash={Hash}",
                correlationId,
                cacheKey
            );

            var payload = new
            {
                model = _model,
                response_format = new { type = "json_object" },
                messages = new[]
                {
                new { role = "system", content = SystemPrompt },
                new { role = "user", content = feedback }
            }
            };

            _log.LogInformation("Sending OpenAI request. CorrelationId={CorrelationId}", correlationId);

            var response = await http.PostAsJsonAsync("v1/chat/completions", payload);

            var duration = DateTime.UtcNow - startTime;
            _log.LogInformation(
                "OpenAI response received. CorrelationId={CorrelationId}, DurationMs={Duration}, StatusCode={Status}",
                correlationId,
                duration.TotalMilliseconds,
                response.StatusCode
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception("OpenAI API error: " + error);
            }

            var result = await response.Content.ReadFromJsonAsync<OpenAIChatResponse>();

            if (result is null || result.Choices.Count == 0)
                throw new Exception("OpenAI returned no choices.");

            var rawJson = result.Choices[0].Message.Content;
            var cleanJson = CleanJson(rawJson);

            FeedbackAnalysisDTO? dto;

            try
            {
                dto = JsonSerializer.Deserialize<FeedbackAnalysisDTO>(
                    cleanJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            catch (Exception ex)
            {
                _log.LogError(
                    ex,
                    "JSON parse error. CorrelationId={CorrelationId}. Using fallback.",
                    correlationId
                );
                dto = Fallback(feedback);
            }

            _cache[cacheKey] = dto!;

            _log.LogInformation("Request finished. CorrelationId={CorrelationId}", correlationId);

            return dto!;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Request failed. CorrelationId={CorrelationId}", correlationId);
            throw;
        }
        finally
        {
            // Reset for the next request
            Correlation.Reset();
        }
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var payload = new
        {
            model = _embeddingModel,
            input = text
        };

        var response = await http.PostAsJsonAsync("v1/embeddings", payload);

        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception("Embedding API Error: " + err);
        }

        var json = await response.Content.ReadFromJsonAsync<EmbeddingResponse>();
        return [.. json!.Data[0].Embedding];
    }

    // Generates a SHA256 hash for consistent cache keys
    private static string Hash(string text)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
        return Convert.ToHexString(bytes);
    }

    // Get only the JSON block
    private static string CleanJson(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "{}";

        string clean = raw
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();

        // Keep only the part between the first '{' and last '}'
        int start = clean.IndexOf('{');
        int end = clean.LastIndexOf('}');
        if (start >= 0 && end >= 0 && end > start)
            clean = clean.Substring(start, end - start + 1);

        return clean;
    }

    // This is if AI fails or gives invalid JSON, so we fallback back to this
    private static FeedbackAnalysisDTO Fallback(string text)
    {
        return new FeedbackAnalysisDTO
        {
            Summary = text.Length > 80 ? text[..80] + "..." : text,
            Sentiment = "Neutral",
            Tags = ["review"],
            Priority = "Low",
            NextAction = "Require manual review."
        };
    }
}
