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
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(clientID);
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("Name cannot be null or empty");
        if (string.IsNullOrWhiteSpace(surname)) throw new ArgumentNullException("Surname cannot be null or empty");

        ClientID = clientID;
        Name = name;
        Surname = surname;
        PhoneNumber = phoneNumber;

    }
    protected Client() { }
    //addCar deleteCar updateCar walidacja dla metod
    public void AddCar(int carID, string model, string make, int year, string vin)
    {
        var carToAdd = _cars.FirstOrDefault(car => car.CarId == carID);
        if (carToAdd != null) throw new ArgumentNullException($"There is car with this ID: {carID} ");

        Car newCar = new Car(carID, model, make, year, vin);
        _cars.Add(newCar);
    }

    public void UpdateCar(int carID, string newMake, string newModel, int newYear)
    {
        var carToUpdate = _cars.FirstOrDefault(car => car.CarId == carID);
        if (carToUpdate == null) throw new ArgumentNullException($"There is no car with this ID: {carID} ");

        carToUpdate.updateCar(newMake, newModel, newYear);
    }

    public void RemoveCar(int carID)
    {
        var carToRemove = _cars.FirstOrDefault(car => car.CarId == carID);
        if (carToRemove == null) throw new ArgumentNullException($"There is no car with this ID: {carID} ");
        
        _cars.Remove(carToRemove);
        
    }
    //usluga zrobiona na aucie (historia) (zmiana wlasicicela)
    public void AddService(int carID, int serviceID, string serviceName, string serviceDescription, int price)
    {
        var carToService = _cars.FirstOrDefault(car => car.CarId == carID);
        var addedService = _services.FirstOrDefault(service => service.ServiceId == serviceID);
        if (carToService == null) throw new ArgumentNullException($"There is no car with this ID: {carID} ");
        
        if (addedService != null) throw new ArgumentException($"There already exists service with this ID: {serviceID}");
        
        Service newService = new Service(serviceID, serviceName, serviceDescription, price);
        _services.Add(newService);

        carToService.IncrementVisits();
    }

    public Car TransferCarOwnership(int carID)
    {
        var carToTransfer = _cars.FirstOrDefault(car => car.CarId == carID);
        if (carToTransfer == null) throw new ArgumentNullException($"There is no car with this ID: {carID} ");

        _cars.Remove(carToTransfer);
        return carToTransfer;
    }

    

}

