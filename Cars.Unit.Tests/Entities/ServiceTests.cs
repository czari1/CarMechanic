using Cars.Domain.Entities;
using Shouldly;

public class ServiceTests
{
    [Fact]
    public void Build_Should_Create_Service_With_Default_Values()
    {
        // Arrange & Act
        var service = new ServiceBuilder()
            .WithDefaults(
                serviceName: "Oil Change",
                serviceDescription: "Standard oil change",
                price: 299.99m)
            .Build();

        // Assert
        service.ShouldNotBeNull();
        service.ServiceName.ShouldBe("Oil Change");
        service.ServiceDescription.ShouldBe("Standard oil change");
        service.Price.ShouldBe(299.99m);
    }

    [Fact]
    public void UpdateService_Should_Change_Name_Description_And_Price()
    {
        // Arrange
        var service = new ServiceBuilder()
            .WithDefaults(
                serviceName: "Oil Change",
                serviceDescription: "Standard oil change",
                price: 150.00m)
            .Build();

        // Act
        service.Update("Premium Oil Change", "Synthetic oil change", 250.00m);

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
            .WithDefaults(
                serviceName: "Oil Change",
                serviceDescription: "Standard oil change",
                price: 150.00m)
            .Build();

        // Act
        service.Update("", "", 150.00m);

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
            .WithDefaults(price: 150.00m)
            .Build();

        // Act
        service.Update("Free Check", "Complimentary inspection", 0m);

        // Assert
        service.Price.ShouldBe(0m);
    }

    [Fact]
    public void UpdateService_Should_Not_Update_Negative_Price()
    {
        // Arrange
        var service = new ServiceBuilder()
            .WithDefaults(price: 150.00m)
            .Build();

        // Act
        service.Update("Oil Change", "Standard service", -50m);

        // Assert
        service.Price.ShouldBe(150.00m);
    }

    [Fact]
    public void Constructor_Should_Set_ServiceDate_To_Current_Time()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var service = new ServiceBuilder()
            .WithDefaults()
            .Build();

        var afterCreation = DateTime.UtcNow;

        // Assert
        service.ServiceDate.ShouldBeGreaterThanOrEqualTo(beforeCreation);
        service.ServiceDate.ShouldBeLessThanOrEqualTo(afterCreation);
    }

    [Theory]
    [InlineData("", "Description", 100)]
    [InlineData("Name", "", 100)]
    public void Constructor_Should_Throw_Exception_When_Invalid_Data(
        string serviceName,
        string serviceDescription,
        decimal price)
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            new Service(serviceName, serviceDescription, price));
    }

    [Fact]
    public void Constructor_Should_Throw_Exception_When_Price_Is_Zero_Or_Negative()
    {
        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() =>
            new Service("Name", "Description", 0));

        Should.Throw<ArgumentOutOfRangeException>(() =>
            new Service("Name", "Description", -1));
    }
}