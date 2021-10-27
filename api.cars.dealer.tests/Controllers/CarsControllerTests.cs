using api.cars.dealer.Controllers;
using api.cars.dealer.Models;
using api.cars.dealer.Services;
using api.cars.dealer.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;
using Microsoft.Extensions.Logging;

namespace api.cars.dealer.Tests.Controllers
{
    public class CarsControllerTests
    {
        private readonly CarsController _controller;
        private readonly ICarServices _service;
        private readonly ILogger<CarsController> _logger;

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

        public CarsControllerTests()    
        {
            _service = Substitute.For<ICarServices>();
            _logger = Substitute.For<ILogger<CarsController>>();
            _controller = new CarsController(_service, _logger);
        }

        [Fact]
        public void ShouldReturnNotFound_When_NoCarsFound()
        {
            // Arrange
            _service.GetCars().ReturnsNull();

            // Act
            var response = _controller.Get();

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<NoContentResult>();
            ((NoContentResult)response).StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.NoContent));
        }

        [Fact]
        public void ShouldReturnData_When_CarsFound()
        {
            // Arrange
            _service.GetCars().Returns(new ApiResult<List<Car>>
            {
                Status = Constants.Success,
                Data = SampleCars
            });

            // Act
            var response = _controller.Get();

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<OkObjectResult>();
            var result = response as OkObjectResult;
            result.StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.OK));
            result.Value.Should().NotBeNull();
            result.Value.Should().BeOfType<ApiResult<List<Car>>>();
        }

        [Fact]
        public void ShouldReturnBadRequestForCreation_When_ApiFailed()
        {
            // Arrange
            _service.AddCar(Arg.Any<Models.Car>()).Returns(new ApiResult<int>
            {
                Status = Constants.Fail
            });

            // Act
            var response = _controller.Post(SampleCars[0]);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)response).StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.BadRequest));
        }

        [Fact]
        public void ShouldReturTrue_When_CarsAddedSuccessfully()
        {
            // Arrange
            _service.AddCar(Arg.Any<Models.Car>()).Returns(new ApiResult<int>
            {
                Status = Constants.Success,
                Data = 1001
            });

            // Act
            var response = _controller.Post(SampleCars[0]);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<OkObjectResult>();
            var result = response as OkObjectResult;
            result.StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.OK));
            result.Value.Should().NotBeNull();
            result.Value.Should().BeOfType<ApiResult<int>>();
        }

        [Fact]
        public void ShouldReturnBadRequestForUpdationg_When_IdNotProvided()
        {
            // Arrange
            SampleCars[0].Id = 0;

            // Act
            var response = _controller.Put(SampleCars[0]);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)response).StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.BadRequest));
        }

        [Fact]
        public void ShouldReturnBadRequestForUpdationg_When_ApiFailed()
        {
            // Arrange
            _service.UpdateCar(Arg.Any<Models.Car>()).Returns(new ApiResult<bool>
            {
                Status = Constants.Fail
            });

            // Act
            var response = _controller.Put(SampleCars[0]);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)response).StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.BadRequest));
        }

        [Fact]
        public void ShouldReturTrue_When_CarsUpdatedSuccessfully()
        {
            // Arrange
            _service.UpdateCar(Arg.Any<Models.Car>()).Returns(new ApiResult<bool>
            {
                Status = Constants.Success,
                Data = true
            });

            // Act
            var response = _controller.Put(SampleCars[0]);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<OkObjectResult>();
            var result = response as OkObjectResult;
            result.StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.OK));
            result.Value.Should().NotBeNull();
            result.Value.Should().BeOfType<ApiResult<bool>>();
        }

        [Fact]
        public void ShouldReturnBadRequestObjectResult_When_InputEmpty()
        {
            // Arrange
            _service.CalculateDiscount(Arg.Any<List<Models.Car>>()).ReturnsNull();

            // Act
            var response = _controller.CalculateDiscount(SampleCars);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)response).StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.BadRequest));
        }

        [Fact]
        public void ShouldReturnBadRequestObjectResult_When_InvlidIdGiven()
        {
            // Arrange
            _service.CalculateDiscount(Arg.Any<List<Models.Car>>()).Returns(new ApiResult<Discount>
            {
                Status = Constants.Fail                
            });

            // Act
            var response = _controller.CalculateDiscount(SampleCars);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)response).StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.BadRequest));
        }

        [Fact]
        public void ShouldReturnDiscount_When_Calculated()
        {
            // Arrange
            _service.CalculateDiscount(Arg.Any<List<Models.Car>>()).Returns(new ApiResult<Discount>
            {
                Status = Constants.Success,
                Data = new Discount
                {
                    DiscountRate = 10,
                    DiscountAmount = 1000,
                    PriceAfterDiscount = 20000
                }
            });

            // Act
            var response = _controller.CalculateDiscount(SampleCars);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<OkObjectResult>();
            var result = response as OkObjectResult;
            result.StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.OK));
            result.Value.Should().NotBeNull();
            result.Value.Should().BeOfType<ApiResult<Discount>>();
        }
    }
}
