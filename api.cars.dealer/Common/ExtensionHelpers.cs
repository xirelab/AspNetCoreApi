namespace api.cars.dealer.Common
{
    public static class ExtensionHelpers
    {
        public static decimal GetDiscount(this decimal amount, int discount)
        {
            return amount * discount / 100;
        }
    }
}
