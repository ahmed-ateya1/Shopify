using E_Shop.Application.Validators.AccountValidator;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace E_Shop.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
            return services;
        }
    }
}
