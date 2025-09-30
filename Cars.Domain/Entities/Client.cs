using Cars.Domain.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace Cars.Domain.Entities;

public class Client : EntityBase
{
    private readonly List<Car> _cars = new();

    public IReadOnlyCollection<Car> Cars => _cars; // Czy chodzilo o to ze klasa Car byla internal zamiast public?
   
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string PhoneNumber { get; private set; }
    


    //konstruktor
    public Client( string name, string surname, string phoneNumber)
    {
        ValidateClientData( name, surname);

        Name = name;
        Surname = surname;
        PhoneNumber = phoneNumber;

    }
    protected Client() { }
    //addCar deleteCar updateCar walidacja dla metod

    private static void ValidateClientData( string name, string surname)
    {
        
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("Name cannot be null or empty", nameof(name));
        
        if (string.IsNullOrWhiteSpace(surname)) throw new ArgumentNullException("Surname cannot be null or empty", nameof(surname));
    }


    public void AddCar( string model, string make, int year, string vin)
    {
        Car newCar = new Car( model, make, year, vin);
        _cars.Add(newCar);
    }

    public void UpdateCar(int id, string newMake, string newModel, int newYear)
    {
        var carToUpdate = GetCarById(id);
        carToUpdate.Update(newMake, newModel, newYear);
    }

    public void RemoveCar(int carID)
    {
        Car carToRemove = GetCarById(carID);        
        _cars.Remove(carToRemove);
        
    }
    //usluga zrobiona na aucie (historia) (zmiana wlasicicela)

    public Car TransferCarOwnership(int id)
    {
        var carToTransfer = GetCarById(id);
        _cars.Remove(carToTransfer);
        return carToTransfer;
    }

    private Car GetCarById(int id)
    {
        return _cars.FirstOrDefault(c => c.Id == id)
            ?? throw new ArgumentException($"Car with ID {Id} not found", nameof(Id));
    }

    

}

