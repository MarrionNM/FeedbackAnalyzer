using FeedbackAnalyzer.Data.Payloads;
using FluentValidation;

namespace FeedbackAnalyzer.Data.Validators;

public class FeedbackValidator : AbstractValidator<FeedbackPayload>
{
    public FeedbackValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty()
            .MinimumLength(5)
            .WithMessage("Feedback text must be at least 5 characters");
    }
}
