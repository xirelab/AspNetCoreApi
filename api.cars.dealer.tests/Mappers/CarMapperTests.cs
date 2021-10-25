using api.cars.dealer.Mappers;
using api.cars.dealer.Models;
using FluentAssertions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Repo = BizCover.Repository.Cars;

namespace api.cars.dealer.Tests.Mappers
{
    public class CarMapperTests
    {
        [Fact]
        public void ShouldReturnFail_When_AddFailed()
        {
            // Arrange
            Task<int> apiResponse = null;

            // Act
            var response = CarMappers.Map(apiResponse);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ApiResult<int>>();
            response.Status.Should().Be("fail");
        }

        [Fact]
        public void ShouldReturnSuccess_When_AddSucceeded()
        {
            // Arrange
            Task<int> apiResponse = Task<int>.Factory.StartNew(() => 10);

            // Act
            var response = CarMappers.Map(apiResponse);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ApiResult<int>>();
            response.Status.Should().Be("success");
            response.Data.Should().Be(10);
        }

        [Fact]
        public void ShouldReturnFail_When_UpdateFailed()
        {
            // Arrange
            Task<bool> apiResponse = null;

            // Act
            var response = CarMappers.Map(apiResponse);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ApiResult<bool>>();
            response.Status.Should().Be("fail");
        }

        [Fact]
        public void ShouldReturnSuccess_When_UpdateSucceeded()
        {
            // Arrange
            Task apiResponse = Task.Factory.StartNew(() => true);

            // Act
            var response = CarMappers.Map(apiResponse);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ApiResult<bool>>();
            response.Status.Should().Be("success");
            response.Data.Should().Be(true);
        }

        private static List<Car> SampleCars = new List<Car>
        {
            new Car
            {
                Id = 12,
                Make = "Make",
                Model = "Model",
                Year = 2021,
                CountryManufactured = "CountryManufactured",
                Colour = "Colour",
                Price = 1000
            },
            new Car
            {
                Id = 22,
                Make = "Make2",
                Model = "Model2",
                Year = 1999,
                CountryManufactured = "CountryManufactured",
                Colour = "Colour",
                Price = 500
            }
        };

        private static List<Repo.Car> RepoSampleCars = new List<Repo.Car>
        {
            new Repo.Car
            {
                Id = 12,
                Make = "Make",
                Model = "Model",
                Year = 1998,
                CountryManufactured = "CountryManufactured",
                Colour = "Colour",
                Price = 1000
            },
            new Repo.Car
            {
                Id = 22,
                Make = "Make2",
                Model = "Model2",
                Year = 1999,
                CountryManufactured = "CountryManufactured",
                Colour = "Colour",
                Price = 500
            },
            new Repo.Car
            {
                Id = 23,
                Make = "Make3",
                Model = "Model3",
                Year = 2019,
                CountryManufactured = "CountryManufactured",
                Colour = "Colour",
                Price = 50000
            }
        };

        [Fact]
        public void ShouldReturnNull_When_EmptyPayload()
        {
            // Arrange
            Task<List<Repo.Car>> apiResponse = Task<List<Repo.Car>>.Factory.StartNew(() => RepoSampleCars);

            // Act
            var response = CarMappers.CalculateDiscount(new List<Car>(), apiResponse);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public void ShouldReturnFail_When_InvalidIdGiven()
        {
            // Arrange
            Task<List<Repo.Car>> apiResponse = Task<List<Repo.Car>>.Factory.StartNew(() => RepoSampleCars);
            SampleCars[0].Id = 33;

            // Act
            var response = CarMappers.CalculateDiscount(SampleCars, apiResponse);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ApiResult<Discount>>();
            response.Status.Should().Be("fail");
        }

        [Fact]
        public void ShouldReturnFail_When_InvalidPriceGiven()
        {
            // Arrange
            Task<List<Repo.Car>> apiResponse = Task<List<Repo.Car>>.Factory.StartNew(() => RepoSampleCars);
            SampleCars[0].Price = 5000;

            // Act
            var response = CarMappers.CalculateDiscount(SampleCars, apiResponse);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ApiResult<Discount>>();
            response.Status.Should().Be("fail");
        }

        [Fact]
        public void ShouldReturnSuccess_When_ValidAndNoDiscount()
        {
            // Arrange
            Task<List<Repo.Car>> apiResponse = Task<List<Repo.Car>>.Factory.StartNew(() => RepoSampleCars);

            // Act
            var response = CarMappers.CalculateDiscount(SampleCars, apiResponse);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ApiResult<Discount>>();
            response.Status.Should().Be("success");
            response.Data.Should().NotBeNull();
            response.Data.DiscountRate.Should().Be(0);
            response.Data.DiscountAmount.Should().Be(0);
            response.Data.PriceAfterDiscount.Should().Be(1500);
        }

        [Fact]
        public void ShouldReturnSuccess_When_ValidAndPriceGreaterThan10000()
        {
            // Arrange
            Task<List<Repo.Car>> apiResponse = Task<List<Repo.Car>>.Factory.StartNew(() => RepoSampleCars);
            SampleCars[0].Price = 50000;
            RepoSampleCars[0].Price = 50000;

            // Act
            var response = CarMappers.CalculateDiscount(SampleCars, apiResponse);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ApiResult<Discount>>();
            response.Status.Should().Be("success");
            response.Data.Should().NotBeNull();
            response.Data.DiscountRate.Should().Be(5);
            response.Data.DiscountAmount.Should().Be(2525);
            response.Data.PriceAfterDiscount.Should().Be(47975);
        }

        [Fact]
        public void ShouldReturnSuccess_When_ValidAndMakeGreaterthan2000()
        {
            // Arrange
            Task<List<Repo.Car>> apiResponse = Task<List<Repo.Car>>.Factory.StartNew(() => RepoSampleCars);
            SampleCars[0].Year= 2020;
            SampleCars[1].Year = 2021;

            // Act
            var response = CarMappers.CalculateDiscount(SampleCars, apiResponse);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ApiResult<Discount>>();
            response.Status.Should().Be("success");
            response.Data.Should().NotBeNull();
            response.Data.DiscountRate.Should().Be(10);
            response.Data.DiscountAmount.Should().Be(150);
            response.Data.PriceAfterDiscount.Should().Be(1350);
        }

        [Fact]
        public void ShouldReturnSuccess_When_ValidAndMorethan2Cars()
        {
            // Arrange
            Task<List<Repo.Car>> apiResponse = Task<List<Repo.Car>>.Factory.StartNew(() => RepoSampleCars);            
            SampleCars.Add(new Car
            {
                Id = 23,
                Make = "Make3",
                Model = "Model3",
                Year = 2019,
                CountryManufactured = "CountryManufactured",
                Colour = "Colour",
                Price = 50000
            });

            // Act
            var response = CarMappers.CalculateDiscount(SampleCars, apiResponse);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ApiResult<Discount>>();
            response.Status.Should().Be("success");
            response.Data.Should().NotBeNull();
            response.Data.DiscountRate.Should().Be(8);
            response.Data.DiscountAmount.Should().Be(4120);
            response.Data.PriceAfterDiscount.Should().Be(47380);
        }

        [Fact]
        public void ShouldReturnSuccess_When_ValidAndMaximumDiscount()
        {
            // Arrange
            Task<List<Repo.Car>> apiResponse = Task<List<Repo.Car>>.Factory.StartNew(() => RepoSampleCars);
            SampleCars[0].Year = 2020;
            SampleCars[1].Year = 2021;
            SampleCars.Add(new Car
            {
                Id = 23,
                Make = "Make3",
                Model = "Model3",
                Year = 2021,
                CountryManufactured = "CountryManufactured",
                Colour = "Colour",
                Price = 50000
            });

            // Act
            var response = CarMappers.CalculateDiscount(SampleCars, apiResponse);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ApiResult<Discount>>();
            response.Status.Should().Be("success");
            response.Data.Should().NotBeNull();
            response.Data.DiscountRate.Should().Be(18);
            response.Data.DiscountAmount.Should().Be(9270);
            response.Data.PriceAfterDiscount.Should().Be(42230);
        }
    }
}
