using Demo.Api.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;

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
        private readonly ICacheService _cacheService;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        public CarService(ApplicationDbContext context, ICacheService cachService, IBackgroundTaskQueue backgroundTaskQueue)
        {
            _context = context;
            _cacheService = cachService;
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        public async Task<IEnumerable<Car>> GetAllAsync(CancellationToken cancellationToken)
        {
            var cars = await _cacheService.GetAsync<IEnumerable<Car>>("cars");
            if (cars != null)
            {
                return cars;
            }
            // to test cashing or retrive form db
            cars = await _context.Cars.ToListAsync(cancellationToken);
            _cacheService.SetAsync("cars", cars);
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

            CashingCars();

            return car;
        }

        private void CashingCars()
        {
            _backgroundTaskQueue.QueueBackgroundWorkItem(async sp =>
            {
                var db = sp.GetRequiredService<ApplicationDbContext>();
                var cars = await db.Cars.ToListAsync();
                if (await _cacheService.ExistsAsync("cars"))
                {
                    await _cacheService.RemoveAsync("cars");
                }
                _cacheService.SetAsync("cars", cars);
            });
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
