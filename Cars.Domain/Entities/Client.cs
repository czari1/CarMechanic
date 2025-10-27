using Cars.Domain.Common.Entities;

namespace Cars.Domain.Entities;

public class Client : AggregateRoot
{
    private readonly List<Car> _cars = new();

    public Client(string name, string surname, string phoneNumber)
    {
        ValidateClientData(name, surname);

        Name = name;
        Surname = surname;
        PhoneNumber = phoneNumber;
        CreatedOn = DateTime.UtcNow;
        ModifiedOn = DateTime.UtcNow;
    }

    protected Client()
    {
        Name = string.Empty;
        Surname = string.Empty;
        PhoneNumber = string.Empty;
    }

    public IReadOnlyCollection<Car> Cars => _cars;

    public string Name { get; private set; }

    public string Surname { get; private set; }

    public string PhoneNumber { get; private set; }

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

    public void Update(int id, string newName, string newSurname, string newPhoneNumber)
    {
        if (!string.IsNullOrWhiteSpace(newName))
        {
            if (newName.Length > 40)
            {
                throw new ArgumentException("Name cannot exceed 40 characters", nameof(newName));
            }
            else
            {
                Name = newName;
            }
        }

        if (!string.IsNullOrWhiteSpace(newSurname))
        {
            if (newSurname.Length > 40)
            {
                throw new ArgumentException("Surname cannot exceed 40 characters", nameof(newSurname));
            }
            else
            {
                Surname = newSurname;
            }
        }

        if (!string.IsNullOrWhiteSpace(newPhoneNumber))
        {
            if (newPhoneNumber.Length != 9)
            {
                throw new ArgumentException("Phone number must be 9 characters", nameof(newPhoneNumber));
            }
            else
            {
                PhoneNumber = newPhoneNumber;
            }
        }
    }

    public Car TransferCarOwnership(int carId)
    {
        var carToTransfer = GetCarById(carId);

        if (carToTransfer.IsDeleted)
        {
            throw new InvalidOperationException($"Cannot transfer car with ID {carId} because it is marked as deleted");
        }

        _cars.Remove(carToTransfer);
        return carToTransfer;
    }

    public void ReceiveCar(Car car)
    {
        if (car == null)
        {
            throw new ArgumentNullException(nameof(car), "Car cannot be null");
        }

        if (car.IsDeleted)
        {
            throw new InvalidOperationException("Cannot receive a car that is marked as deleted");
        }

        if (_cars.Any(c => c.VIN == car.VIN))
        {
            throw new InvalidOperationException($"Car with VIN {car.VIN} already belongs to this client");
        }

        _cars.Add(car);
    }

    private Car GetCarById(int id)
    {
        return _cars.FirstOrDefault(c => c.Id == id)
            ?? throw new ArgumentException($"Car with ID {id} not found", nameof(id));
    }

    private static void ValidateClientData(string name, string surname)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException("Name cannot be null or empty", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(surname))
        {
            throw new ArgumentNullException("Surname cannot be null or empty", nameof(surname));
        }
    }
}