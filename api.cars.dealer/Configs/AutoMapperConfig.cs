using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using api.cars.dealer.Models;
using Repo = BizCover.Repository.Cars;

namespace api.cars.dealer.Configs
{
    public static class AutoMapperConfig
    {
        public static void RegisterMapper(this IServiceCollection services)
        {
            services.TryAddSingleton(provider =>
            {
                var mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Repo.Car, Car>();
                    cfg.CreateMap<Car, Repo.Car>();
                }).CreateMapper();

                mapper.ConfigurationProvider.AssertConfigurationIsValid();
                return mapper;
            });
        }
    }
}
