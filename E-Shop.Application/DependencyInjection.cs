using E_Shop.Application.Dtos.ProductDtos;
using E_Shop.Application.ServicesContract;
using E_Shop.Domain.Models;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace E_Shop.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Configure Mapster mappings
            TypeAdapterConfig<Product, ProductResponse>.NewConfig()
                .Map(dest => dest.ImageUrls, src => src.Images.Select(i => i.ImageUrl));

            services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
            services.AddScoped<IFileServices, FileService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IMemoryCacheService, MemoryCacheService>();
            services.AddScoped<IShoppingCartService, ShoppingCartService>();

            services.AddDistributedMemoryCache();
            services.AddMemoryCache();

            return services;
        }
    }
}
