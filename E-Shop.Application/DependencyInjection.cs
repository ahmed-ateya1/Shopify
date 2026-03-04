using Microsoft.Extensions.DependencyInjection;

namespace E_Shop.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
            services.AddScoped<IFileServices,FileService>();
            services.AddScoped<ICategoryService,CategoryService>();
            services.AddScoped<IProductImageService,ProductImageService>();

            return services;
        }
    }
}
