using api.cars.dealer.Common;
using api.cars.dealer.Models;
using api.cars.dealer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repo = BizCover.Repository.Cars;

namespace api.cars.dealer.Mappers
{
    public static class CarMappers
    {
        public static ApiResult<int> Map(Task<int> apiResponse)
        {
            if (apiResponse == null || apiResponse.Exception != null)
            {
                return new ApiResult<int>
                {
                    Status = Constants.Fail,
                    Message = apiResponse?.Exception?.Message
                };
            }
            return new ApiResult<int>
            {
                Status = Constants.Success,
                Data = apiResponse.Result
            };
        }

        public static ApiResult<bool> Map(Task apiResponse)
        {
            if (apiResponse == null || apiResponse.Exception != null)
            {
                return new ApiResult<bool>
                {
                    Status = Constants.Fail,
                    Message = apiResponse?.Exception?.Message
                };
            }
            return new ApiResult<bool>
            {
                Status = Constants.Success,
                Data = true
            };
        }

        public static ApiResult<Discount> CalculateDiscount(List<Car> cars, Task<List<Repo.Car>> liveCars)
        {
            if (cars == null || cars.Count == 0) return null;

            var prices = from x in cars
                         join y in liveCars.Result on x.Id equals y.Id
                         where x.Price == y.Price   // comparing price with server value
                         select x.Price;

            if (prices.Count() != cars.Count)
            {
                return new ApiResult<Discount>
                {
                    Status = Constants.Fail,
                    Message = "Invlid Input Details"
                };
            }

            var total = prices.Sum(); var discount = 0;

            if (total > 10000)
            {
                discount += 5;
            }

            if (cars.Count > 2)
            {
                discount += 3;
            }

            if (cars.Find(x => x.Year <= 2000) == null)
            {
                discount += 10;
            }

            return new ApiResult<Discount>
            {
                Status = Constants.Success,
                Data = new Discount
                {
                    DiscountRate = discount,
                    DiscountAmount = total.GetDiscount(discount),
                    PriceAfterDiscount = total - total.GetDiscount(discount)
                }
            };
        }
    }
}
