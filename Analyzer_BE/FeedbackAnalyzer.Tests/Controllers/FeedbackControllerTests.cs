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

namespace FeedbackAnalyzer.Tests.Controllers;

public class FeedbackControllerTests
{
    private readonly Mock<IFeedbackRepository> _feedbackRepo = new();
    private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly Mock<IAnalysisService> _analysisService = new();
    private readonly Mock<ILogger<FeedbackController>> _logger = new();

    private FeedbackController BuildController()
    {
        return new FeedbackController(
            _feedbackRepo.Object,
            _tagRepo.Object,
            _analysisService.Object,
            _logger.Object
        );
    }

    [Fact]
    public async Task Create_ReturnsCreatedAt_WhenPayloadValid()
    {
        // Arrange
        var payload = new FeedbackPayload { Message = "I love this AI platform that analyzes my product reviews is great", Email = "john@smith.com" };

        var analysisDto = new FeedbackAnalysisDTO
        {
            Summary = "good",
            Sentiment = "Positive",
            Priority = "Low",
            NextAction = "Keep it",
            Tags = ["ux"]
        };

        // analysis feedback on the AI service
        _analysisService
            .Setup(x => x.AnalyzeAsync(payload.Message))
            .ReturnsAsync(analysisDto);

        // map the feedback data
        var saved = new FeedbackDTO
        {
            Id = Guid.NewGuid().ToString(),
            Text = payload.Message,
            Email = payload.Email,
            Analysis = analysisDto
        };

        _feedbackRepo
            .Setup(r => r.CreateFeedback(It.IsAny<FeedbackDTO>()))
            .ReturnsAsync(saved);

        // tag repo returns a tag.
        _tagRepo
            .Setup(t => t.GetOrCreateTagAsync(It.IsAny<string>()))
            .ReturnsAsync(new TagDTO { Id = "tag1", Name = "ux" });

        _tagRepo
            .Setup(t => t.AttachFeedbackTagAsync(saved.Id, It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _feedbackRepo
            .Setup(r => r.GetFeedbackById(saved.Id))
            .ReturnsAsync(saved);

        var controller = BuildController();

        // Act
        var result = await controller.Create(payload);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var created = result as CreatedAtActionResult;
        created!.ActionName.Should().Be(nameof(FeedbackController.GetById));
        var returned = created.Value as FeedbackDTO;
        returned!.Id.Should().Be(saved.Id);
    }

    [Fact]
    public async Task Create_ThrowsBadRequest_WhenMessageMissing()
    {
        // Arrange
        var controller = BuildController();
        var payload = new FeedbackPayload { Message = "", Email = null };

        // Act & Assert: The controller throws BadRequestException for empty message per your controller code
        await Assert.ThrowsAsync<BadRequestException>(() => controller.Create(payload));
    }
}
