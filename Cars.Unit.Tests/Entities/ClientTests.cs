using Cars.Domain.Entities;
using Cars.Tests.Builders;
using Shouldly;

public class ClientTests
{
    [Fact]
    public void Build_Should_Create_Client_With_Default_Values()
    {
        // Arrange & Act
        var client = new ClientBuilder()
            .WithDefaults(
                name: "Jan",
                surname: "Kowalski",
                phoneNumber: "123456789")
            .Build();

        // Assert
        client.ShouldNotBeNull();
        client.Name.ShouldBe("Jan");
        client.Surname.ShouldBe("Kowalski");
        client.PhoneNumber.ShouldBe("123456789");
        client.Cars.ShouldBeEmpty();
        client.IsDeleted.ShouldBeFalse();
    }

    [Fact]
    public void AddCar_Should_Add_Car_To_Collection()
    {
        // Arrange
        var client = new ClientBuilder()
            .WithDefaults()
            .Build();

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
            .WithDefaults()
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
        var client = new ClientBuilder()
            .WithDefaults()
            .Build();

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            client.UpdateCar(999, "Honda", "Civic", 2023));
    }

    [Fact]
    public void RemoveCar_Should_Remove_Car_From_Collection()
    {
        // Arrange
        var client = new ClientBuilder()
            .WithDefaults()
            .WithCar("Toyota", "Corolla", 2020, "1HGBH41JXMN109186")
            .Build();
        var carId = client.Cars.First().Id;

        // Act
        client.RemoveCar(carId);

        // Assert
        client.Cars.ShouldBeEmpty();
    }

    [Fact]
    public void Update_Should_Change_Client_Properties()
    {
        // Arrange
        var client = new ClientBuilder()
            .WithDefaults(name: "Jan", surname: "Kowalski", phoneNumber: "123456789")
            .Build();

        // Act
        client.Update(client.Id, "Anna", "Nowak", "987654321");

        // Assert
        client.Name.ShouldBe("Anna");
        client.Surname.ShouldBe("Nowak");
        client.PhoneNumber.ShouldBe("987654321");
    }

    [Fact]
    public void TransferCarOwnership_Should_Remove_And_Return_Car()
    {
        // Arrange
        var client = new ClientBuilder()
            .WithDefaults()
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
        var client = new ClientBuilder()
            .WithDefaults()
            .Build();

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            client.TransferCarOwnership(999));
    }

    [Fact]
    public void TransferCarOwnership_Should_Throw_When_Car_Is_Deleted()
    {
        // Arrange
        var client = new ClientBuilder()
            .WithDefaults()
            .WithCar()
            .Build();
        var car = client.Cars.First();
        car.Delete(car.Id);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
            client.TransferCarOwnership(car.Id));
    }

    [Fact]
    public void ReceiveCar_Should_Add_Car_To_Client()
    {
        // Arrange
        var client1 = new ClientBuilder()
            .WithDefaults(name: "Jan")
            .WithCar(vin: "1HGBH41JXMN109186")
            .Build();
        var client2 = new ClientBuilder()
            .WithDefaults(name: "Anna")
            .Build();
        var carId = client1.Cars.First().Id;

        // Act
        var car = client1.TransferCarOwnership(carId);
        client2.ReceiveCar(car);

        // Assert
        client1.Cars.ShouldBeEmpty();
        client2.Cars.Count.ShouldBe(1);
        client2.Cars.First().VIN.ShouldBe("1HGBH41JXMN109186");
    }

    [Fact]
    public void ReceiveCar_Should_Throw_Exception_When_Car_Is_Null()
    {
        // Arrange
        var client = new ClientBuilder()
            .WithDefaults()
            .Build();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => client.ReceiveCar(null!));
    }

    [Fact]
    public void ReceiveCar_Should_Throw_Exception_When_VIN_Already_Exists()
    {
        // Arrange
        var client = new ClientBuilder()
            .WithDefaults()
            .WithCar(vin: "1HGBH41JXMN109186")
            .Build();
        var duplicateCar = new CarBuilder()
            .WithDefaults(vin: "1HGBH41JXMN109186")
            .Build();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => client.ReceiveCar(duplicateCar));
    }

    [Theory]
    [InlineData("", "Kowalski")]
    [InlineData("Jan", "")]
    public void Constructor_Should_Throw_Exception_When_Invalid_Data(
        string name,
        string surname)
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            new Client(name, surname, "123456789"));
    }
}