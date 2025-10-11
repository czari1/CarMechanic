using Cars.Domain.Common.Entities;

namespace Cars.Domain.Entities;

public class Client : AggregateRoot
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


    public void AddCar(string make, string model, int year, string vin)
    {
        Car newCar = new Car(make, model, year, vin);
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

    public void Update(string newName, string newSurname, string newPhoneNumber)
    {
        if (!string.IsNullOrWhiteSpace(newName))
        {
            if (newName.Length > 40)
                throw new ArgumentException("Name cannot exceed 40 characters", nameof(newName));
            Name = newName;
        }

        if (!string.IsNullOrWhiteSpace(newSurname))
        {
            if (newSurname.Length > 40)
                throw new ArgumentException("Surname cannot exceed 40 characters", nameof(newSurname));
            Surname = newSurname;
        }

        if (!string.IsNullOrWhiteSpace(newPhoneNumber))
        {
            if (newPhoneNumber.Length != 9)
                throw new ArgumentException("Phone number must be 9 characters", nameof(newPhoneNumber));
            PhoneNumber = newPhoneNumber;
        }
    }
    public Car TransferCarOwnership(int id)
    {
        var carToTransfer = GetCarById(id);
        _cars.Remove(carToTransfer);
        return carToTransfer;
    }

    private Car GetCarById(int id)
    {
        return _cars.FirstOrDefault(c => c.Id == id)
            ?? throw new ArgumentException($"Car with ID {id} not found", nameof(id));
    }

    

}

