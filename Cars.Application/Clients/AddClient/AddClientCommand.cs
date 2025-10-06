using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.Application.Clients.AddClient
{
    public sealed record AddClientCommand(string Name, string Surname, string PhoneNumber)
    : IRequest<int>;

    // Dodac pola od dodwania klienta
    
}
