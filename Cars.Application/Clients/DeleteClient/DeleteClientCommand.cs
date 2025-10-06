using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.Application.Clients.DeleteClient
{
    public sealed record DeleteClientCommand(int Id)
    : IRequest;
}
