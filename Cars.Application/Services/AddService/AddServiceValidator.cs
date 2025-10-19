using Cars.Application.Common;
using FluentValidation;

namespace Cars.Application.Services.AddService;

public sealed class AddServiceValidator : AbstractValidator<AddServiceCommand>
{

    public AddServiceValidator(ICarContext context)
    {

        RuleFor(x => x.ServiceName)
            .NotEmpty().WithMessage("Service name is required")
            .MaximumLength(50).WithMessage("Service name cannot exceed 50 characters");

        RuleFor(x => x.ServiceDescription)
            .NotEmpty().WithMessage("Service description is required")
            .MaximumLength(200).WithMessage("Service description cannot exceed 200 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be bigger than 0");
    }
}
