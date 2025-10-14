using FluentValidation;

namespace Cars.Application.Clients.AddClient;

public sealed class AddClientValidator : AbstractValidator<AddClientCommand>
{
    public AddClientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Surname)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Length(9)
            .Matches(@"^\d{9}$");

        //dodac reszte pol
    }
}
