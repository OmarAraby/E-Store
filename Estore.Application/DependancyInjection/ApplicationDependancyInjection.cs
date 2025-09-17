using Estore.Application.Utiles.Mapping;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Estore.Application.DependancyInjection
{
    public static class ApplicationDependancyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));

            // validations
            services.AddValidatorsFromAssembly(typeof(ApplicationDependancyInjection).Assembly);

            // app services

            return services;
        }
    }
}
