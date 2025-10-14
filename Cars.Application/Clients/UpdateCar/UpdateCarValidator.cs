using FluentValidation;

namespace Cars.Application.Clients.UpdateCar;

public sealed class UpdateCarValidator : AbstractValidator<UpdateCarCommand>
{
    public UpdateCarValidator()
    {
        RuleFor(x => x.NewMake)
            .MaximumLength(20).WithMessage("Make cannot exceed 20 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.NewMake));

        RuleFor(x => x.NewModel)
            .MaximumLength(20).WithMessage("Model cannot exceed 20 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.NewModel));

        RuleFor(x => x.NewYear)
            .GreaterThan(1900).WithMessage("Year must be grater than 1900")
            .LessThanOrEqualTo(DateTime.Now.Year + 1)
            .WithMessage($"Year cannot be greater than {DateTime.Now.Year + 1}");
    }
}
