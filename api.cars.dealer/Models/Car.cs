using System.ComponentModel.DataAnnotations;

namespace api.cars.dealer.Models
{
    public class Car
    {
        public int Id { get; set; }
        [Required]
        public string Make { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public string CountryManufactured { get; set; }
        [Required]
        public string Colour { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
