using Cars.Application.Common;
using FluentValidation;

namespace Cars.Application.Clients.AddCar;

public sealed class AddCarValidator : AbstractValidator<AddCarCommand>
{
    private readonly ICarContext _context;

    public AddCarValidator(ICarContext context)
    {
        _context = context;

        RuleFor(x => x.Make)
            .NotEmpty().WithMessage("Make is required")
            .MaximumLength(20).WithMessage("Make cannot exceed 20 characters");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required")
            .MaximumLength(20).WithMessage("Model cannot exceed 20 characters");

        RuleFor(x => x.Year)
            .GreaterThan(1900).WithMessage("Year must be grater than 1900")
            .LessThanOrEqualTo(DateTime.Now.Year + 1)
            .WithMessage($"Year cannot be greater than {DateTime.Now.Year + 1}");

        RuleFor(x => x.Vin)
            .NotEmpty().WithMessage("VIN is required")
            .Length(17).WithMessage("VIN must be exactly 17 characters")
            .Matches("^[A-HJ-NPR-Z0-9]{17}$").WithMessage("VIN must contain only valid characters (A-Z, 0-9, excluding I, O, Q)")
            .Must(BeUniqueVIN).WithMessage("Vin already exists in the system");
    }

    private bool BeUniqueVIN(string vin)
    {
        return !_context.CarQuery.Any(c => c.VIN == vin);
    }
}
