using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.Application.Clients.AddClient
{
    public sealed class AddClientValidator : AbstractValidator<AddClientCommand>
    {
        public AddClientValidator()
        {
            RuleFor(x => x.Name).NotEmpty().EmailAddress();
            RuleFor(x => x.Surname).NotEmpty().MaximumLength(100);
            RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(100);
            //dodac reszte pol
        }
    }
}
