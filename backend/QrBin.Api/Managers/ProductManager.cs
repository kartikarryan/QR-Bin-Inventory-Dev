using FluentValidation;
using QrBin.Api.Common.Utility;
using QrBin.Data.Entities;
using QrBin.Data.Repositories;
using QrBin.ViewModels;
using QrBin.ViewModels.Products;

namespace QrBin.Api.Managers;

public interface IProductManager
{
    Task<ApiResponse<List<ProductResponse>>> GetAllAsync(int organizationId, string? search, CancellationToken cancellationToken);
    Task<ApiResponse<ProductResponse>> GetByIdAsync(int organizationId, int productId, CancellationToken cancellationToken);
    Task<ApiResponse<ProductResponse>> CreateAsync(int organizationId, CreateProductRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<ProductResponse>> UpdateAsync(int organizationId, int productId, UpdateProductRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<ProductResponse>> AddStockAsync(int organizationId, int productId, AddStockRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<List<StockMovementResponse>>> GetStockHistoryAsync(int organizationId, int productId, CancellationToken cancellationToken);
}

public class ProductManager : IProductManager
{
    private readonly IProductRepository _productRepository;
    private readonly IValidator<CreateProductRequest> _createValidator;
    private readonly IValidator<UpdateProductRequest> _updateValidator;
    private readonly IValidator<AddStockRequest> _addStockValidator;
    private readonly IApiResponseBuilder _response;

    public ProductManager(
        IProductRepository productRepository,
        IValidator<CreateProductRequest> createValidator,
        IValidator<UpdateProductRequest> updateValidator,
        IValidator<AddStockRequest> addStockValidator,
        IApiResponseBuilder response)
    {
        _productRepository = productRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _addStockValidator = addStockValidator;
        _response = response;
    }

    public async Task<ApiResponse<List<ProductResponse>>> GetAllAsync(int organizationId, string? search, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(organizationId, search, cancellationToken);
        return _response.Ok(products.Select(ToResponse).ToList());
    }

    public async Task<ApiResponse<ProductResponse>> GetByIdAsync(int organizationId, int productId, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(organizationId, productId, cancellationToken);
        if (product is null)
            return _response.NotFound<ProductResponse>("Product not found.");

        return _response.Ok(ToResponse(product));
    }

    public async Task<ApiResponse<ProductResponse>> CreateAsync(int organizationId, CreateProductRequest request, CancellationToken cancellationToken)
    {
        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return _response.BadRequest<ProductResponse>(
                null,
                validation.Errors.First().ErrorMessage,
                validation.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var code = string.IsNullOrWhiteSpace(request.Code) ? null : request.Code.Trim();
        if (code is not null && await _productRepository.CodeExistsAsync(organizationId, code, null, cancellationToken))
            return _response.Conflict<ProductResponse>($"Product code '{code}' is already in use.");

        var product = new Product
        {
            OrganizationId = organizationId,
            Name = request.Name.Trim(),
            Code = code,
            Unit = request.Unit.Trim(),
            SellingPrice = request.SellingPrice,
            CurrentStock = request.OpeningStock
        };

        await _productRepository.AddAsync(product, cancellationToken);

        if (request.OpeningStock > 0)
        {
            await _productRepository.AddStockMovementAsync(new StockMovement
            {
                OrganizationId = organizationId,
                Product = product,
                Reason = StockMovementReason.OpeningStock,
                QuantityDelta = request.OpeningStock,
                PreviousStock = 0,
                NewStock = request.OpeningStock
            }, cancellationToken);
        }

        await _productRepository.SaveChangesAsync(cancellationToken);

        return _response.Created(ToResponse(product), "Product created");
    }

    public async Task<ApiResponse<ProductResponse>> UpdateAsync(int organizationId, int productId, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return _response.BadRequest<ProductResponse>(
                null,
                validation.Errors.First().ErrorMessage,
                validation.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var product = await _productRepository.GetTrackedByIdAsync(organizationId, productId, cancellationToken);
        if (product is null)
            return _response.NotFound<ProductResponse>("Product not found.");

        var code = string.IsNullOrWhiteSpace(request.Code) ? null : request.Code.Trim();
        if (code is not null && await _productRepository.CodeExistsAsync(organizationId, code, productId, cancellationToken))
            return _response.Conflict<ProductResponse>($"Product code '{code}' is already in use.");

        product.Name = request.Name.Trim();
        product.Code = code;
        product.Unit = request.Unit.Trim();
        product.SellingPrice = request.SellingPrice;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.SaveChangesAsync(cancellationToken);

        return _response.Ok(ToResponse(product), "Product updated");
    }

    public async Task<ApiResponse<ProductResponse>> AddStockAsync(int organizationId, int productId, AddStockRequest request, CancellationToken cancellationToken)
    {
        var validation = await _addStockValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return _response.BadRequest<ProductResponse>(
                null,
                validation.Errors.First().ErrorMessage,
                validation.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var product = await _productRepository.GetTrackedByIdAsync(organizationId, productId, cancellationToken);
        if (product is null)
            return _response.NotFound<ProductResponse>("Product not found.");

        var previousStock = product.CurrentStock;
        var newStock = previousStock + request.Quantity;

        product.CurrentStock = newStock;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.AddStockMovementAsync(new StockMovement
        {
            OrganizationId = organizationId,
            ProductId = product.Id,
            Reason = StockMovementReason.StockReceived,
            QuantityDelta = request.Quantity,
            PreviousStock = previousStock,
            NewStock = newStock
        }, cancellationToken);

        await _productRepository.SaveChangesAsync(cancellationToken);

        return _response.Ok(ToResponse(product), "Stock updated");
    }

    public async Task<ApiResponse<List<StockMovementResponse>>> GetStockHistoryAsync(int organizationId, int productId, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(organizationId, productId, cancellationToken);
        if (product is null)
            return _response.NotFound<List<StockMovementResponse>>("Product not found.");

        var movements = await _productRepository.GetStockHistoryAsync(organizationId, productId, cancellationToken);

        return _response.Ok(movements.Select(m => new StockMovementResponse
        {
            Id = m.Id,
            Reason = ReasonLabel(m.Reason),
            QuantityDelta = m.QuantityDelta,
            NewStock = m.NewStock,
            BillId = m.BillId,
            CreatedAt = m.CreatedAt
        }).ToList());
    }

    private static string ReasonLabel(StockMovementReason reason) => reason switch
    {
        StockMovementReason.OpeningStock => "Opening Stock",
        StockMovementReason.Sale => "Sold",
        StockMovementReason.StockReceived => "Stock Received",
        _ => reason.ToString()
    };

    private static ProductResponse ToResponse(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Code = product.Code,
        Unit = product.Unit,
        SellingPrice = product.SellingPrice,
        CurrentStock = product.CurrentStock
    };
}
