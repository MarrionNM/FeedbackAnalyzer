using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FeedbackAnalyzer.Controllers;
using FeedbackAnalyzer.Contracts.IRepository;
using FeedbackAnalyzer.Contracts.IServices;
using FeedbackAnalyzer.Data.DTO;
using FeedbackAnalyzer.Data.Payloads;
using FeedbackAnalyzer.Helpers.Exceptions;
using FluentValidation;
using FeedbackAnalyzer.Data.Response;

namespace FeedbackAnalyzer.Tests.Controllers;

public class FeedbackControllerTests
{
    private readonly Mock<IFeedbackRepository> _feedbackRepo = new();
    private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly Mock<IValidator<FeedbackPayload>> _validator = new();
    private readonly Mock<IAnalysisService> _analysisService = new();
    private readonly Mock<ILogger<FeedbackController>> _logger = new();

    private FeedbackController BuildController()
    {
        return new FeedbackController(
            _feedbackRepo.Object,
            _tagRepo.Object,
            _validator.Object,
            _analysisService.Object,
            _logger.Object
        );
    }

    [Fact]
    public async Task Create_ReturnsOk_WhenPayloadValid()
    {
        // Arrange
        var payload = new FeedbackPayload
        {
            Message = "I love this AI platform",
            Email = "john@smith.com"
        };

        // Validator should accept the payload
        _validator
            .Setup(v => v.Validate(payload))
            .Returns(new FluentValidation.Results.ValidationResult());

        var analysisDto = new FeedbackAnalysisDTO
        {
            Summary = "good",
            Sentiment = "Positive",
            Priority = "Low",
            NextAction = "Keep it",
            Tags = ["ux"]
        };

        _analysisService
            .Setup(x => x.AnalyzeAsync(payload.Message))
            .ReturnsAsync(analysisDto);

        // Embedding expected in controller
        float[] embedding = [0.1f, 0.2f, 0.3f];
        _analysisService
            .Setup(x => x.GenerateEmbeddingAsync(payload.Message))
            .ReturnsAsync(embedding);

        var saved = new FeedbackDTO
        {
            Id = Guid.NewGuid().ToString(),
            Text = payload.Message,
            Email = payload.Email,
            Analysis = new FeedbackAnalysisDTO
            {
                Summary = analysisDto.Summary,
                Sentiment = analysisDto.Sentiment,
                Priority = analysisDto.Priority,
                NextAction = analysisDto.NextAction
            }
        };

        _feedbackRepo
            .Setup(r => r.CreateFeedback(It.IsAny<FeedbackDTO>(), embedding))
            .ReturnsAsync(saved);

        _tagRepo
            .Setup(t => t.GetOrCreateTagAsync("ux"))
            .ReturnsAsync(new TagDTO { Id = "tag1", Name = "ux" });

        _tagRepo
            .Setup(t => t.AttachFeedbackTagAsync(saved.Id, "tag1"))
            .Returns(Task.CompletedTask);

        _feedbackRepo
            .Setup(r => r.GetFeedbackById(saved.Id))
            .ReturnsAsync(saved);

        var controller = BuildController();

        // Act
        var result = await controller.Create(payload);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var ok = result as OkObjectResult;

        var response = ok!.Value as Response<FeedbackDTO>;
        response!.IsSuccess.Should().BeTrue();
        response.Data.Id.Should().Be(saved.Id);

        // Ensure service calls occurred
        _analysisService.Verify(a => a.AnalyzeAsync(payload.Message), Times.Once);
        _analysisService.Verify(a => a.GenerateEmbeddingAsync(payload.Message), Times.Once);
        _tagRepo.Verify(t => t.GetOrCreateTagAsync("ux"), Times.Once);
    }

    [Fact]
    public async Task Create_ThrowsBadRequest_WhenMessageMissing()
    {
        // Arrange
        var controller = BuildController();
        var payload = new FeedbackPayload { Message = "", Email = null };

        // Validation for empty message
        _validator
            .Setup(v => v.Validate(payload))
            .Returns(new FluentValidation.Results.ValidationResult());

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => controller.Create(payload));
    }
}
