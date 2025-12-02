using FeedbackAnalyzer.Contracts.IRepository;
using FeedbackAnalyzer.Contracts.IServices;
using FeedbackAnalyzer.Data.DTO;
using FeedbackAnalyzer.Data.Payloads;
using FeedbackAnalyzer.Data.Response;
using FeedbackAnalyzer.Helpers.Exceptions;
using FeedbackAnalyzer.Helpers.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FeedbackAnalyzer.Controllers;

public class FeedbackController(
    IFeedbackRepository feedbackRepository,
    ITagRepository tagRepository,
    IValidator<FeedbackPayload> validator,
    IAnalysisService analysisService, ILogger<FeedbackController> logger
    ) : ApiBaseController
{
    /// <summary>
    /// POST /api/feedback
    /// Creates feedback and analyze through AI model
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(FeedbackPayload feedbackPayload)
    {
        validator.ValidateAndThrowValidationException(feedbackPayload);

        logger.LogInformation("Received feedback submission from Email={Email}", feedbackPayload.Email);

        if (string.IsNullOrWhiteSpace(feedbackPayload.Message))
            throw new BadRequestException("Feedback text is required.");

        // Run OpenAI analysis
        var analysis = await analysisService.AnalyzeAsync(feedbackPayload.Message);

        // Generate vector embedding
        var vectorEmbedding = await analysisService.GenerateEmbeddingAsync(feedbackPayload.Message);

        // Prepare DTO
        var feedback = new FeedbackDTO
        {
            Text = feedbackPayload.Message.Trim(),
            Email = feedbackPayload.Email,
            CreatedAt = DateTime.UtcNow,
            Analysis = new FeedbackAnalysisDTO
            {
                Summary = analysis.Summary,
                Sentiment = analysis.Sentiment,
                Priority = analysis.Priority,
                NextAction = analysis.NextAction
            }
        };

        var savedFeedback = await feedbackRepository.CreateFeedback(feedback, vectorEmbedding);

        // tags
        foreach (var tagName in analysis.Tags)
        {
            var tag = await tagRepository.GetOrCreateTagAsync(tagName);
            await tagRepository.AttachFeedbackTagAsync(savedFeedback.Id, tag.Id);
        }

        var fullFeedback = await feedbackRepository.GetFeedbackById(savedFeedback.Id);

        return Ok(new Response<FeedbackDTO>
        {
            IsSuccess = true,
            Data = fullFeedback
        });
    }

    /// <summary>
    /// GET /api/feedback
    /// PGett all of the Feedbacks in a paged response and filterd query params
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] Filter filter)
    {
        var feedbacks = await feedbackRepository.GetFeedbacks(filter);
        return Ok(new Response<PagedResponse<FeedbackDTO>>() { Data = feedbacks, IsSuccess = true });
    }

    /// <summary>
    /// GET /api/feedback/{id}
    /// Gett feedback full etails by feedback id
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var item = await feedbackRepository.GetFeedbackById(id);
        if (item == null)
            return NotFound(new Response<FeedbackDTO> { Message = "Feedback not found", IsSuccess = false });

        return Ok(new Response<FeedbackDTO>
        {
            IsSuccess = true,
            Data = item
        });
    }
}
