using AutoMapper;
using api.cars.dealer.Models;
using api.cars.dealer.Services;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Repo = BizCover.Repository.Cars;
using api.cars.dealer.Common;

namespace api.cars.dealer.Tests.Services
{
    public class CarServicesTests
    {
        private readonly CarServices _service;
        private readonly Repo.ICarRepository _carRepository;
        private readonly IMapper _autoMapper;
        private readonly IErrorHandler<CarServices> _errorHandler;

        private static List<Repo.Car> RepoSampleCars = new List<Repo.Car>
        {
            new Repo.Car
            {
                Id = 12,
                Make = "Make",
                Model = "Model",
                Year = 2010,
                CountryManufactured = "CountryManufactured",
                Colour = "Colour",
                Price = 1000
            }
        };
        private static List<Car> SampleCars = new List<Car>
        {
            new Car
            {
                Id = 12,
                Make = "Make",
                Model = "Model",
                Year = 2010,
                CountryManufactured = "CountryManufactured",
                Colour = "Colour",
                Price = 1000
            }
        };

        public CarServicesTests()
        {
            _carRepository = Substitute.For<Repo.ICarRepository>();
            _autoMapper = Substitute.For<IMapper>();
            _errorHandler = Substitute.For<IErrorHandler<CarServices>>();
            _service = new CarServices(_carRepository, _autoMapper, _errorHandler);
        }

        [Fact]
        public void ShouldReturnNull_When_NoCarsFound()
        {
            // Arrange
            _carRepository.GetAllCars().ReturnsNull();

            // Act
            var response = _service.GetCars();

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public void ShouldReturnCars_When_CarsFound()
        {
            // Arrange
            _carRepository.GetAllCars().Returns(RepoSampleCars);
            _autoMapper.Map<List<Car>>(Arg.Any<List<Repo.Car>>()).Returns(SampleCars);

            // Act
            var response = _service.GetCars();

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ApiResult<List<Car>>>();
            response.Status.Should().Be("success");
        }

        [Fact]
        public void ShouldReturnFalse_When_AddReturnedError()
        {
            // Arrange
            _carRepository.Add(Arg.Any<Repo.Car>()).ReturnsNull();

            // Act
            var response = _service.AddCar(SampleCars[0]);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ApiResult<int>>();
            response.Status.Should().Be("fail");
        }

        [Fact]
        public void ShouldReturnTrue_When_AddSuccessful()
        {
            // Arrange
            _carRepository.Add(Arg.Any<Repo.Car>()).Returns(1001);

            // Act
            var response = _service.AddCar(SampleCars[0]);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ApiResult<int>>();
            response.Status.Should().Be("success");
        }

        [Fact]
        public void ShouldReturnFalse_When_UpdateReturnedError()
        {
            // Arrange
            _carRepository.Update(Arg.Any<Repo.Car>()).ReturnsNull();

            // Act
            var response = _service.UpdateCar(SampleCars[0]);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ApiResult<bool>>();
            response.Status.Should().Be("fail");
        }

        [Fact]
        public void ShouldReturnTrue_When_UpdateSuccessful()
        {
            // Act
            var response = _service.UpdateCar(SampleCars[0]);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ApiResult<bool>>();
            response.Status.Should().Be("success");
        }

        [Fact]
        public void ShouldReturnNull_When_RequestBodyIsNull()
        {
            // Arrange
            _carRepository.GetAllCars().Returns(RepoSampleCars);

            // Act
            var response = _service.CalculateDiscount(new List<Car>());

            // Assert
            response.Should().BeNull();            
        }

        [Fact]
        public void ShouldReturnFail_When_InvalidIdPassed()
        {
            // Arrange
            _carRepository.GetAllCars().Returns(RepoSampleCars);

            // Act
            var response = _service.CalculateDiscount(new List<Models.Car>
            {
                new Models.Car
                {
                    Id = 22,
                    Make = "Make",
                    Model = "Model",
                    Year = 2010,
                    CountryManufactured = "CountryManufactured",
                    Colour = "Colour",
                    Price = 1000
                }
            });

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ApiResult<Discount>>();
            response.Status.Should().Be("fail");
        }

        [Fact]
        public void ShouldReturnSuccess_When_Valid()
        {
            // Arrange
            _carRepository.GetAllCars().Returns(RepoSampleCars);

            // Act
            var response = _service.CalculateDiscount(SampleCars);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ApiResult<Discount>>();
            response.Status.Should().Be("success");
        }
    }
}
