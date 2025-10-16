using FluentValidation;

namespace Cars.Application.Clients.UpdateClient;

public sealed class UpdateClientValidator : AbstractValidator<UpdateClientCommand>
{
    public UpdateClientValidator()
    {
        RuleFor(x => x.NewId)
            .GreaterThan(0);

        RuleFor(x => x.NewName)
            .MaximumLength(100);

        RuleFor(x => x.NewSurname)
            .MaximumLength(100);

        RuleFor(x => x.NewPhoneNumber)
            .Length(9);

        //dodac reszte pol
    }
}
