using Cars.Integration.Application.Tests.Base;
using System.Reflection.Metadata;

namespace Cars.Integration.Application.Tests.Client.DisplayAllClients;
public sealed class DisplayAllClientsCommandHandlerTests : ApplicationTestsBase
{
    [Trait("Category", "IntegrationTests")]

    public async Task Handle_Should_Display_All_Not_Deleted_Clients()
    {

    }
}
