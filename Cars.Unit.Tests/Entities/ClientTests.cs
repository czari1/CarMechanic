using Cars.Tests.Builders;
using Shouldly;

public class ClientTests
{
    [Fact]
    public void AddCar_Should_Add_Car_To_Collection()
    {
        // Arrange
        var client = new ClientBuilder().Build();

        // Act
        client.AddCar("Toyota", "Corolla", 2020, "1HGBH41JXMN109186");

        // Assert
        client.Cars.Count.ShouldBe(1);
        client.Cars.First().Make.ShouldBe("Toyota");
        client.Cars.First().Model.ShouldBe("Corolla");
    }

    [Fact]
    public void UpdateCar_Should_Update_Existing_Car()
    {
        // Arrange
        var client = new ClientBuilder()
            .WithCar("Toyota", "Corolla", 2020, "1HGBH41JXMN109186")
            .Build();
        var carId = client.Cars.First().Id;

        // Act
        client.UpdateCar(carId, "Honda", "Civic", 2023);

        // Assert
        client.Cars.First().Make.ShouldBe("Honda");
        client.Cars.First().Model.ShouldBe("Civic");
        client.Cars.First().Year.ShouldBe(2023);
    }

    [Fact]
    public void UpdateCar_Should_Throw_When_Car_Not_Found()
    {
        // Arrange
        var client = new ClientBuilder().Build();

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            client.UpdateCar(999, "Honda", "Civic", 2023));
    }

    [Fact]
    public void RemoveCar_Should_Remove_Car_From_Collection()
    {
        // Arrange
        var client = new ClientBuilder()
            .WithCar("Toyota", "Corolla", 2020, "1HGBH41JXMN109186")
            .Build();
        var carId = client.Cars.First().Id;

        // Act
        client.RemoveCar(carId);

        // Assert
        client.Cars.ShouldBeEmpty();
    }

    [Fact]
    public void TransferCarOwnership_Should_Remove_And_Return_Car()
    {
        // Arrange
        var client = new ClientBuilder()
            .WithCar("Toyota", "Corolla", 2020, "1HGBH41JXMN109186")
            .Build();
        var carId = client.Cars.First().Id;

        // Act
        var transferredCar = client.TransferCarOwnership(carId);

        // Assert
        transferredCar.ShouldNotBeNull();
        transferredCar.Make.ShouldBe("Toyota");
        client.Cars.ShouldBeEmpty();
    }

    [Fact]
    public void TransferCarOwnership_Should_Throw_When_Car_Not_Found()
    {
        // Arrange
        var client = new ClientBuilder().Build();

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            client.TransferCarOwnership(999));
    }
}