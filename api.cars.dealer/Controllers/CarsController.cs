using System.Collections.Generic;
using api.cars.dealer.Common;
using api.cars.dealer.Models;
using api.cars.dealer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.cars.dealer.Controllers
{
    [Route("api/cars")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ICarServices _service;
        private readonly ILogger<CarsController> _logger;  
        public CarsController(ICarServices services, ILogger<CarsController> logger)
        {
            _service = services;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Car), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get()
        {
            _logger.LogInformation("CarsController.Get method called!!!");
            var response  =  _service.GetCars();
            return response?.Data == null || response.Data.Count == 0 ? NoContent() : Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Post([FromBody]Car car)
        {
            var response = _service.AddCar(car);
            return (response?.Status ?? Constants.Fail) == Constants.Fail ? BadRequest(response) : Ok(response);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Put([FromBody] Car car)
        {
            if ((car?.Id ?? 0) <= 0)
            {
                return BadRequest("Id is a mandatory field for update");
            }
            var response = _service.UpdateCar(car);
            return (response?.Status ?? Constants.Fail) == Constants.Fail ? BadRequest(response) : Ok(response);
        }

        [HttpPost]
        [Route("discount")]
        [ProducesResponseType(typeof(Discount), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CalculateDiscount([FromBody] List<Car> cars)
        {
            var response = _service.CalculateDiscount(cars);
            return (response?.Status ?? Constants.Fail) == Constants.Fail ? BadRequest(response) : Ok(response);
        }
    }
}
