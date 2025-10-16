using Cars.Domain.Common.Entities;
using Cars.Domain.Common.Interfaces;

namespace Cars.Domain.Entities;

public class Car : EntityBase, ISoftDeleteTable
{
    public Car(string make, string model, int year, string vin)
    {
        ValidateCarData(make, model, year, vin);

        Make = make;
        Model = model;
        Year = year;
        VIN = vin; // z malych
        Visits = 0;
        IsDeleted = false;
    }

    protected Car()
    {
        Make = string.Empty;
        Model = string.Empty;
        VIN = string.Empty;
    }

    public string Make { get; private set; }

    public string Model { get; private set; }

    public int Year { get; private set; }

    public string VIN { get; private set; } // czy uzyc Vin jako id jest unikalny

    public int Visits { get; private set; }

    public bool IsDeleted { get; set; }

    //Konstruktor pola + protected bez paramterowy kazda encja
    //Metoda updateCar co miala robic?
    public void Update(string newMake, string newModel, int newYear) // metody z duzej
    {
        if (!string.IsNullOrWhiteSpace(newMake))
        {
            Make = newMake;
        }

        if (!string.IsNullOrWhiteSpace(newModel))
        {
            Model = newModel;
        }

        if (newYear >= 1900 && newYear <= DateTime.Now.Year + 1)
        {
            Year = newYear;
        }
        else
        {
            throw new ArgumentException($"Invalid year: {newYear}", nameof(newYear));
        }
    }

    public void IncrementVisits()
    {
        Visits++;
    }

    //metoda delete zmiana flagi
    public void Delete(int id)
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
        }
    }

    //metoda reverse zmiana flagi spowrotem
    public void ReverseDelete(int id)
    {
        if (IsDeleted)
        {
            IsDeleted = false;
        }
    }

    private static void ValidateCarData(string make, string model, int year, string vin)
    {
        if (string.IsNullOrWhiteSpace(make))
        {
            throw new ArgumentNullException("Make cannot be null", nameof(make));
        }

        if (string.IsNullOrWhiteSpace(model))
        {
            throw new ArgumentNullException("Model cannot be null", nameof(model));
        }

        if (year <= 1900 || year > DateTime.Now.Year + 1)
        {
            throw new ArgumentOutOfRangeException("Year of production is not possible", nameof(year));
        }

        if (string.IsNullOrWhiteSpace(vin) || vin.Length != 17)
        {
            throw new ArgumentOutOfRangeException("VIN cannot be null and have to be 17 characters", nameof(vin));
        }
    }
}
