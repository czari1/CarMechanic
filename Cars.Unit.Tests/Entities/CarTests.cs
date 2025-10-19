using Cars.Domain.Entities;
using Cars.Tests.Builders;
using Shouldly;

public class CarTests
{
    [Fact]
    public void Build_Should_Create_Car_With_Default_Values()
    {
        // Arrange & Act
        var car = new CarBuilder()
            .WithDefaults(
                make: "Toyota",
                model: "Corolla",
                year: 2023,
                vin: "1HGBH41JXMN109186")
            .Build();

        // Assert
        car.ShouldNotBeNull();
        car.Make.ShouldBe("Toyota");
        car.Model.ShouldBe("Corolla");
        car.Year.ShouldBe(2023);
        car.VIN.ShouldBe("1HGBH41JXMN109186");
        car.Visits.ShouldBe(0);
        car.IsDeleted.ShouldBeFalse();
    }

    [Fact]
    public void Update_Should_Change_Make_Model_And_Year()
    {
        // Arrange
        var car = new CarBuilder()
            .WithDefaults(make: "Toyota", model: "Corolla", year: 2020)
            .Build();

        // Act
        car.Update("Honda", "Civic", 2023);

        // Assert
        car.Make.ShouldBe("Honda");
        car.Model.ShouldBe("Civic");
        car.Year.ShouldBe(2023);
    }

    [Fact]
    public void Update_Should_Not_Change_Values_When_Empty_Strings_Provided()
    {
        // Arrange
        var car = new CarBuilder()
            .WithDefaults(make: "Toyota", model: "Corolla")
            .Build();
        var originalMake = car.Make;
        var originalModel = car.Model;

        // Act
        car.Update("", "", 2020);

        // Assert
        car.Make.ShouldBe(originalMake);
        car.Model.ShouldBe(originalModel);
    }

    [Fact]
    public void Update_Should_Throw_When_Invalid_Year()
    {
        // Arrange
        var car = new CarBuilder()
            .WithDefaults()
            .Build();

        // Act & Assert
        Should.Throw<ArgumentException>(() => car.Update("Honda", "Civic", 1800));
    }

    [Fact]
    public void IncrementVisits_Should_Increase_Visits_Count()
    {
        // Arrange
        var car = new CarBuilder()
            .WithDefaults()
            .Build();

        // Act
        car.IncrementVisits();
        car.IncrementVisits();
        car.IncrementVisits();

        // Assert
        car.Visits.ShouldBe(3);
    }

    [Fact]
    public void Delete_Should_Set_IsDeleted_To_True()
    {
        // Arrange
        var car = new CarBuilder()
            .WithDefaults()
            .Build();

        // Act
        car.Delete(1);

        // Assert
        car.IsDeleted.ShouldBeTrue();
    }

    [Fact]
    public void ReverseDelete_Should_Set_IsDeleted_To_False()
    {
        // Arrange
        var car = new CarBuilder()
            .WithDefaults()
            .WithDeleted(true)
            .Build();

        // Act
        car.ReverseDelete(1);

        // Assert
        car.IsDeleted.ShouldBeFalse();
    }

    [Theory]
    [InlineData("", "Model", 2020, "1HGBH41JXMN109186")]
    [InlineData("Make", "", 2020, "1HGBH41JXMN109186")]
    [InlineData("Make", "Model", 2020, "")]
    [InlineData("Make", "Model", 2020, "SHORT")]
    public void Constructor_Should_Throw_Exception_When_Invalid_Data(
        string make,
        string model,
        int year,
        string vin)
    {
        // Act & Assert
        Should.Throw<Exception>(() => new Car(make, model, year, vin));
    }
}