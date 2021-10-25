using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using api.cars.dealer.Common;
using api.cars.dealer.Mappers;
using api.cars.dealer.Models;
using Repo = BizCover.Repository.Cars;

namespace api.cars.dealer.Services
{
    public interface ICarServices
    {
        ApiResult<List<Car>> GetCars();
        ApiResult<int> AddCar(Car car);
        ApiResult<bool> UpdateCar(Car car);
        ApiResult<Discount> CalculateDiscount(List<Car> cars);
    }

    public class CarServices : ICarServices
    {
        private readonly Repo.ICarRepository _carRepository;
        private readonly IMapper _autoMapper;
        private readonly IErrorHandler<CarServices> _errorHandler;

        public CarServices(
            Repo.ICarRepository carRepository, 
            IMapper autoMapper,
            IErrorHandler<CarServices> errorHandler)
        {
            _carRepository = carRepository;
            _autoMapper = autoMapper;
            _errorHandler = errorHandler;
        }

        public ApiResult<List<Car>> GetCars()
        {
            _errorHandler.LogTrace("Starting Get Cars..");
            var cars = _carRepository.GetAllCars();
            _errorHandler.LogTrace("Completed Get Cars..");

            return cars?.Result != null && cars.Result.Any() ? new ApiResult<List<Car>>
            {
                Status = Constants.Success,
                Data = _autoMapper.Map<List<Car>>(cars.Result)
            } : null;
        }

        public ApiResult<int> AddCar(Car car)
        {
            var apiResponse = _carRepository.Add(_autoMapper.Map<Repo.Car>(car));

            return CarMappers.Map(apiResponse);
        }

        public ApiResult<bool> UpdateCar(Car car)
        {
            var apiResponse = _carRepository.Update(_autoMapper.Map<Repo.Car>(car));

            return CarMappers.Map(apiResponse);
        }

        public ApiResult<Discount> CalculateDiscount(List<Car> cars)
        {
            var liveCars = _carRepository.GetAllCars();

            return CarMappers.CalculateDiscount(cars, liveCars);
        }
    }
}
