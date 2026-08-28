using FluentValidation;
using QrBin.Api.Common.Utility;
using QrBin.Api.Managers;
using QrBin.Api.Services;
using QrBin.Validators;
using QrBin.ViewModels.Billing;
using QrBin.ViewModels.Inventory;
using QrBin.ViewModels.Products;

namespace QrBin.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddQrBinApi(this IServiceCollection services, IConfiguration configuration)
    {
        // Utility
        services.AddScoped<IApiResponseBuilder, ApiResponseBuilder>();
        services.AddScoped<ICurrentOrganizationContext, CurrentOrganizationContext>();

        // Managers
        services.AddScoped<IDashboardManager, DashboardManager>();
        services.AddScoped<IInventoryManager, InventoryManager>();
        services.AddScoped<IPartManager, PartManager>();
        services.AddScoped<IProductManager, ProductManager>();
        services.AddScoped<IBillManager, BillManager>();

        // Validators
        services.AddScoped<IValidator<CreatePartRequest>, CreatePartRequestValidator>();
        services.AddScoped<IValidator<UpdatePartRequest>, UpdatePartRequestValidator>();
        services.AddScoped<IValidator<StockMovementRequest>, StockMovementRequestValidator>();
        services.AddScoped<IValidator<CreateProductRequest>, CreateProductRequestValidator>();
        services.AddScoped<IValidator<UpdateProductRequest>, UpdateProductRequestValidator>();
        services.AddScoped<IValidator<AddStockRequest>, AddStockRequestValidator>();
        services.AddScoped<IValidator<CreateBillRequest>, CreateBillRequestValidator>();

        return services;
    }
}
