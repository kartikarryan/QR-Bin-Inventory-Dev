using FluentValidation;
using QrBin.Api.Common.Options;
using QrBin.Api.Common.Utility;
using QrBin.Api.Managers;
using QrBin.Api.Services;
using QrBin.Validators;
using QrBin.ViewModels.Auth;
using QrBin.ViewModels.Inventory;

namespace QrBin.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddQrBinApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // Utility
        services.AddScoped<IApiResponseBuilder, ApiResponseBuilder>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentManagerContext, CurrentManagerContext>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // Managers
        services.AddScoped<IAuthManager, AuthManager>();
        services.AddScoped<IDashboardManager, DashboardManager>();
        services.AddScoped<IInventoryManager, InventoryManager>();
        services.AddScoped<IPartManager, PartManager>();

        // Validators
        services.AddScoped<IValidator<ManagerLoginRequest>, ManagerLoginRequestValidator>();
        services.AddScoped<IValidator<CreatePartRequest>, CreatePartRequestValidator>();
        services.AddScoped<IValidator<UpdatePartRequest>, UpdatePartRequestValidator>();
        services.AddScoped<IValidator<StockMovementRequest>, StockMovementRequestValidator>();

        return services;
    }
}
