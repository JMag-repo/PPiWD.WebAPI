using Microsoft.EntityFrameworkCore;
using PPiWD.WebAPI.Database;

namespace PPiWD.WebAPI.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPooledDbContextFactory<DatabaseContext>(options =>
            options.UseNpgsql(configuration.GetValue<string>("DatabaseConnectionString"))
                .UseLazyLoadingProxies());

        return services;
    }
}