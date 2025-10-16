using Shouldly;

namespace Cars.Unit.Tests.Entities;
public class ServiceTests
{
    [Fact]
    public void UpdateService_Should_Change_Name_Description_And_Price()
    {
        // Arrange
        var service = new ServiceBuilder()
            .WithServiceName("Oil Change")
            .WithServiceDescription("Standard oil change")
            .WithPrice(150.00m)
            .Build();

        // Act
        service.UpdateService("Premium Oil Change", "Synthetic oil change", 250.00m);

        // Assert
        service.ServiceName.ShouldBe("Premium Oil Change");
        service.ServiceDescription.ShouldBe("Synthetic oil change");
        service.Price.ShouldBe(250.00m);
    }

    [Fact]
    public void UpdateService_Should_Not_Change_Values_When_Empty_Strings_Provided()
    {
        // Arrange
        var service = new ServiceBuilder()
            .WithServiceName("Oil Change")
            .WithServiceDescription("Standard oil change")
            .WithPrice(150.00m)
            .Build();

        // Act
        service.UpdateService("", "", 150.00m);

        // Assert
        service.ServiceName.ShouldBe("Oil Change");
        service.ServiceDescription.ShouldBe("Standard oil change");
        service.Price.ShouldBe(150.00m);
    }

    [Fact]
    public void UpdateService_Should_Update_Price_To_Zero()
    {
        // Arrange
        var service = new ServiceBuilder()
            .WithPrice(150.00m)
            .Build();

        // Act
        service.UpdateService("Free Check", "Complimentary inspection", 0m);

        // Assert
        service.Price.ShouldBe(0m);
    }

    [Fact]
    public void UpdateService_Should_Not_Update_Negative_Price()
    {
        // Arrange
        var service = new ServiceBuilder()
            .WithPrice(150.00m)
            .Build();

        // Act
        service.UpdateService("Oil Change", "Standard service", -50m);

        // Assert
        service.Price.ShouldBe(150.00m); // Price should remain unchanged
    }
}
