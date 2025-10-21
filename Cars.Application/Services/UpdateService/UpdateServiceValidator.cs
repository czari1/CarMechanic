using FluentValidation;

namespace Cars.Application.Services.UpdateService;

public sealed class UpdateServiceValidator : AbstractValidator<UpdateServiceCommand>
{
    public UpdateServiceValidator()
    {
        RuleFor(x => x.ServiceId)
            .GreaterThan(0).WithMessage("Service ID must be greater than 0");

        RuleFor(x => x.NewServiceName)
            .MaximumLength(50).WithMessage("Service name cannot exceed 50 characters");

        RuleFor(x => x.NewServiceDescription)
            .MaximumLength(200).WithMessage("Service description cannot exceed 200 characters");

        RuleFor(x => x.NewPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0");
    }
}