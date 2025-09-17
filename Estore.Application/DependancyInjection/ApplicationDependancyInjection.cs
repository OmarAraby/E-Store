using Estore.Application.Interfaces;
using Estore.Application.Services;
using Estore.Application.Utiles.HandleFiles;
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
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            services.AddScoped<IFileService, FileService>();




            return services;
        }
    }
}
