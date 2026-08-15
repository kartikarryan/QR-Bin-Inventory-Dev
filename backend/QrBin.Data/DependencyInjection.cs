using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QrBin.Data.Context;
using QrBin.Data.Repositories;

namespace QrBin.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddQrBinData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<QrBinDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IPartRepository, PartRepository>();

        return services;
    }
}
