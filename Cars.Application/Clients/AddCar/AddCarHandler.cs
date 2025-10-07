using Cars.Application.Common;
using MediatR;

namespace Cars.Application.Clients.AddCar
{
    public sealed class AddCarHandler(ICarContext context)
    : IRequestHandler<AddCarCommand, int>
    {
        public async Task<int> Handle(AddCarCommand cmd, CancellationToken ct)
        {
            var validator = new AddCarValidator(context);
            var result = await validator.ValidateAsync(cmd, ct);
            
            if (!result.IsValid) return 0;

            var client = context.Clients.FirstOrDefault(c => c.Id == cmd.ClientId);

            if (client == null) return 0;

            client.AddCar(cmd.Make, cmd.Model, cmd.Year, cmd.Vin);
            await context.SaveChangesAsync(ct);

            var addedCar = client.Cars.FirstOrDefault(c => c.VIN == cmd.Vin);
            
            return addedCar?.Id ?? 0;


        }
    }
}
