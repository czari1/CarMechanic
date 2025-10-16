using Shouldly;

namespace Cars.Unit.Tests.Entities;

public class CarTests
{
    [Fact]
    public void Update_Should_Change_Make_Model_And_Year()
    {
        // Arrange
        var car = new CarBuilder()
            .WithMake("Toyota")
            .WithModel("Corolla")
            .WithYear(2020)
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
            .WithMake("Toyota")
            .WithModel("Corolla")
            .Build();

        // Act
        car.Update("", "", 2020);

        // Assert
        car.Make.ShouldBe("Toyota");
        car.Model.ShouldBe("Corolla");
    }

    [Fact]
    public void Update_Should_Throw_When_Invalid_Year()
    {
        // Arrange
        var car = new CarBuilder().Build();

        // Act & Assert
        Should.Throw<ArgumentException>(() => car.Update("Honda", "Civic", 1800));
    }

    [Fact]
    public void IncrementVisits_Should_Increase_Visits_Count()
    {
        // Arrange
        var car = new CarBuilder().Build();

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
        var car = new CarBuilder().Build();

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
            .WithDeleted(true)
            .Build();

        // Act
        car.ReverseDelete(1);

        // Assert
        car.IsDeleted.ShouldBeFalse();
    }
}

