namespace api.cars.dealer.Models
{
    public class Discount
    {
        public int DiscountRate { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal PriceAfterDiscount { get; set; }
    }
}
