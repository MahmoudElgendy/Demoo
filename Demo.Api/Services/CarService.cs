using Demo.Api.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace Demo.Api.Services
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetAllAsync(CancellationToken cancellationToken);
        Task<Car?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Car> CreateAsync(Car car, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(Guid id, Car car, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
    public class CarService : ICarService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICachService _cachService;

        public CarService(ApplicationDbContext context, ICachService cachService)
        {
            _context = context;
            _cachService = cachService;
        }

        public async Task<IEnumerable<Car>> GetAllAsync(CancellationToken cancellationToken)
        {
            var cars = _cachService.GetDate<IEnumerable<Car>>("cars");
            if (cars != null)
            {
                return cars;
            }
            // to test cashing or retrive form db
            cars = await _context.Cars.ToListAsync(cancellationToken);
            _cachService.SetData("cars",cars);
            await Task.Delay(TimeSpan.FromSeconds(10));
            return cars;
        }

        public async Task<Car?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Cars.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<Car> CreateAsync(Car car, CancellationToken cancellationToken)
        {
            car.Id = Guid.NewGuid();

            _context.Cars.Add(car);
            await _context.SaveChangesAsync(cancellationToken);

            return car;
        }

        public async Task<bool> UpdateAsync(Guid id, Car car, CancellationToken cancellationToken)
        {
            var existing = await _context.Cars.FindAsync(new object[] { id }, cancellationToken);
            if (existing is null)
                return false;

            existing.Name = car.Name;
            existing.Make = car.Make;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var car = await _context.Cars.FindAsync(new object[] { id }, cancellationToken);
            if (car is null)
                return false;

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

}
