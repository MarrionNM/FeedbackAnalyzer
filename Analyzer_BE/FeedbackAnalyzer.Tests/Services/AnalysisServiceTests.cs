using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using FeedbackAnalyzer.Data.Services;

namespace FeedbackAnalyzer.Tests.Services;

public class AnalysisServiceTests
{
    private static HttpClient CreateHttpClientMock(string responseContent, HttpStatusCode status = HttpStatusCode.OK)
    {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        handlerMock
           .Protected()
           .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = status,
               Content = new StringContent(responseContent, Encoding.UTF8, "application/json"),
           })
           .Verifiable();

        var client = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://fake-openai.test/")
        };

        return client;
    }

    private static AnalysisService CreateService(HttpClient client)
    {
        var inMemConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OpenAI:Model"] = "gpt-test-model"
            })
            .Build();

        var logger = Mock.Of<ILogger<AnalysisService>>();

        return new AnalysisService(client, inMemConfig, logger);
    }

    [Fact]
    public async Task AnalyzeAsync_ParsesValidJson_ReturnsDto()
    {
        // Arrange: Build OpenAI-like wrapper with JSON string inside message.content
        var innerJson = JsonSerializer.Serialize(new
        {
            summary = "User likes UI",
            sentiment = "Positive",
            tags = new[] { "ux", "usability" },
            priority = "Low",
            nextAction = "Keep doing UI improvements"
        });

        var openAiPayload = new
        {
            choices = new[] {
                new {
                    message = new {
                        content = innerJson
                    }
                }
            }
        };

        var responseBody = JsonSerializer.Serialize(openAiPayload);

        var http = CreateHttpClientMock(responseBody, HttpStatusCode.OK);
        var service = CreateService(http);

        // Act
        var result = await service.AnalyzeAsync("Some feedback text");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("User likes UI", result.Summary);
        Assert.Equal("Positive", result.Sentiment);
        Assert.Contains("ux", result.Tags);
        Assert.Equal("Low", result.Priority);
        Assert.Equal("Keep doing UI improvements", result.NextAction);
    }

    [Fact]
    public async Task AnalyzeAsync_InvalidJson_UsesFallback()
    {
        // Arrange: OpenAI responds with invalid JSON inside content
        var openAiPayload = new
        {
            choices = new[] {
                new {
                    message = new {
                        content = "this is not JSON"
                    }
                }
            }
        };

        var responseBody = JsonSerializer.Serialize(openAiPayload);

        var http = CreateHttpClientMock(responseBody, HttpStatusCode.OK);
        var service = CreateService(http);

        // Act
        var result = await service.AnalyzeAsync("This is a feedback text used to create fallback summary");

        // Assert: fallback returns Neutral / manual-review / Low etc per your Fallback implementation
        Assert.NotNull(result);
        Assert.Equal("Neutral", result.Sentiment);
        Assert.Contains("review", result.Tags);
        Assert.Equal("Low", result.Priority);
        Assert.False(string.IsNullOrWhiteSpace(result.Summary));
    }

    [Fact]
    public async Task AnalyzeAsync_OpenAiReturnsError_Throws()
    {
        // Arrange: OpenAI returns 400
        var http = CreateHttpClientMock("{\"error\":\"bad request\"}", HttpStatusCode.BadRequest);
        var service = CreateService(http);

        // Act & Assert
        await Assert.ThrowsAsync<System.Exception>(() => service.AnalyzeAsync("anything"));
    }
}
