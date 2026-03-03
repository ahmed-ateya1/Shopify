using E_Shop.Domain.Models.Identity;
using E_Shop.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E_Shop.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services
            , IConfiguration configuration)
        {
            services.AddDbContext<EShopDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("EshopConnection"));
            });
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 5;
            })
           .AddEntityFrameworkStores<EShopDbContext>()
           .AddDefaultTokenProviders()
           .AddUserStore<UserStore<ApplicationUser, ApplicationRole, EShopDbContext, Guid>>()
           .AddRoleStore<RoleStore<ApplicationRole, EShopDbContext, Guid>>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });
            return services;
        }
    }
}
