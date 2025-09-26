using Cars.Domain.Common.Entities;
using Cars.Domain.Common.Interfaces;

namespace Cars.Domain.Entities;

public class Car : EntityBase, ISoftDeleteTable
{
    //Dane dla auta sety prywatne protected
    public int CarId { get; private set; }
    public string Make { get; private set; }
    public string Model { get; private set; }
    public int Year { get; private set; }
    public string VIN { get; private set; } // czy uzyc Vin jako id jest unikalny
    public int Visits { get; private set; }
    public bool IsDeleted { get; set; }

    //Konstruktor pola + protected bez paramterowy kazda encja
    public Car(int carId, string make, string model, int year, string Vin)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(carId);
        if (string.IsNullOrWhiteSpace(make)) throw new ArgumentNullException("Make cannot be null");
        if (string.IsNullOrWhiteSpace(model)) throw new ArgumentNullException("Model cannot be null");
        if (year <= 1900 || year > DateTime.Now.Year + 1 ) throw new ArgumentOutOfRangeException("Year of production is not possible");
        if (string.IsNullOrWhiteSpace(Vin) || Vin.Length != 17) throw new ArgumentOutOfRangeException("VIN cannot be null and have to be 17 characters");
        
        CarId = carId;
        Make = make;
        Model = model;
        Year = year;
        VIN = Vin;
        Visits = 0;
        IsDeleted = false;
    }
    protected Car() {}

    //Metoda updateCar co miala robic?
    public void updateCar(string newMake, string newModel, int newYear) 
    {
        if (!string.IsNullOrWhiteSpace(newMake)) Make = newMake;
        if (!string.IsNullOrWhiteSpace(newModel)) Model = newModel;
        if (newYear <= 1900 || newYear > DateTime.Now.Year + 1) throw new ArgumentOutOfRangeException("Year od production not possible");
        Year = newYear;
    }

    public void IncrementVisits()
    {
        Visits++;
    }

    //metoda delete zmiana flagi
    public void DeleteCar(int CarId)
    {
        if (!IsDeleted) IsDeleted = true;
        
    }
    //metoda reverse zmiana flagi spowrotem
    public void ReverseCarDelete(int CarId)
    {
        if (IsDeleted) IsDeleted = false;
    }
}
