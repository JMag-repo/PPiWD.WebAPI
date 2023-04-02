using PPiWD.WebAPI.Services;
using PPiWD.WebAPI.Services.Interfaces;

namespace PPiWD.WebAPI.Extensions;

public static class AppServicesExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}