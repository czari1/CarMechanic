using Cars.Domain.Common.Entities;

namespace Cars.Domain.Entities;

public class Client : EntityBase
{
    private readonly List<Car> _cars = new();

    public IReadOnlyCollection<Car> Cars => _cars; // Czy chodzilo o to ze klasa Car byla internal zamiast public?

    private readonly List<Service> _services = new();
    public IReadOnlyCollection<Service> Services => _services;

    //dane klienta dodaj
    public int ClientID { get; private set; }
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string PhoneNumber { get; private set; }


    //konstruktor
    public Client(int clientID, string name, string surname, string phoneNumber)
    {
        ValidateClientData(clientID, name, surname);

        ClientID = clientID;
        Name = name;
        Surname = surname;
        PhoneNumber = phoneNumber;

    }
    protected Client() { }
    //addCar deleteCar updateCar walidacja dla metod

    private static void ValidateClientData(int clientID, string name, string surname)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(clientID);
        
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("Name cannot be null or empty", nameof(name));
        
        if (string.IsNullOrWhiteSpace(surname)) throw new ArgumentNullException("Surname cannot be null or empty", nameof(surname));
    }

    public void AddCar(string model, string make, int year, string vin)
    {
        var carId = _cars.Any() ? _cars.Max(c => c.CarId) + 1 : 1;
        AddCar(carId, model, make, year, vin);
    }

    public void AddCar(int carID, string model, string make, int year, string vin)
    {
        var carToAdd = _cars.FirstOrDefault(car => car.CarId == carID);
        if (carToAdd != null) throw new ArgumentNullException($"There is car with this ID: {carID} ");

        Car newCar = new Car(carID, model, make, year, vin);
        _cars.Add(newCar);
    }

    public void UpdateCar(int carID, string newMake, string newModel, int newYear)
    {
        var carToUpdate = GetCarById(carID);
        carToUpdate.updateCar(newMake, newModel, newYear);
    }

    public void RemoveCar(int carID)
    {
        Car carToRemove = GetCarById(carID);        
        _cars.Remove(carToRemove);
        
    }
    //usluga zrobiona na aucie (historia) (zmiana wlasicicela)

    public void AddService(int carID, string serviceName, string serviceDescription, int price)
    {
        var serviceId = _services.Any() ? _services.Max(s => s.ServiceId) + 1 : 1;
        AddService(carID, serviceId, serviceName, serviceDescription, price);
    }

    public void AddService(int carID, int serviceID, string serviceName, string serviceDescription, int price)
    {
        var car = GetCarById(carID);

        if (_services.Any(s => s.ServiceId == serviceID))
            throw new InvalidOperationException($"Service with ID {serviceID} already exists");

        var newService = new Service(serviceID, serviceName, serviceDescription, price);
        _services.Add(newService);
        car.IncrementVisits();
    }

    public Car TransferCarOwnership(int carID)
    {
        var carToTransfer = GetCarById(carID);
        _cars.Remove(carToTransfer);
        return carToTransfer;
    }

    private Car GetCarById(int carID)
    {
        return _cars.FirstOrDefault(c => c.CarId == carID)
            ?? throw new ArgumentException($"Car with ID {carID} not found", nameof(carID));
    }

    

}

