using FluentValidation;
using QrBin.Api.Common.Utility;
using QrBin.Data.Entities;
using QrBin.Data.Repositories;
using QrBin.ViewModels;
using QrBin.ViewModels.Billing;

namespace QrBin.Api.Managers;

public interface IBillManager
{
    Task<ApiResponse<List<BillSummaryResponse>>> GetAllAsync(int organizationId, CancellationToken cancellationToken);
    Task<ApiResponse<BillResponse>> GetByIdAsync(int organizationId, int billId, CancellationToken cancellationToken);
    Task<ApiResponse<BillResponse>> CreateAsync(int organizationId, CreateBillRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<BillResponse>> CreateReturnAsync(int organizationId, int billId, CreateBillReturnRequest request, CancellationToken cancellationToken);
}

public class BillManager : IBillManager
{
    private readonly IBillRepository _billRepository;
    private readonly IProductRepository _productRepository;
    private readonly IValidator<CreateBillRequest> _validator;
    private readonly IValidator<CreateBillReturnRequest> _returnValidator;
    private readonly IApiResponseBuilder _response;

    public BillManager(
        IBillRepository billRepository,
        IProductRepository productRepository,
        IValidator<CreateBillRequest> validator,
        IValidator<CreateBillReturnRequest> returnValidator,
        IApiResponseBuilder response)
    {
        _billRepository = billRepository;
        _productRepository = productRepository;
        _validator = validator;
        _returnValidator = returnValidator;
        _response = response;
    }

    public async Task<ApiResponse<List<BillSummaryResponse>>> GetAllAsync(int organizationId, CancellationToken cancellationToken)
    {
        var bills = await _billRepository.GetAllAsync(organizationId, cancellationToken);
        return _response.Ok(bills.Select(ToSummary).ToList());
    }

    public async Task<ApiResponse<BillResponse>> GetByIdAsync(int organizationId, int billId, CancellationToken cancellationToken)
    {
        var bill = await _billRepository.GetByIdAsync(organizationId, billId, cancellationToken);
        if (bill is null)
            return _response.NotFound<BillResponse>("Bill not found.");

        return _response.Ok(ToResponse(bill));
    }

    public async Task<ApiResponse<BillResponse>> CreateAsync(int organizationId, CreateBillRequest request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return _response.BadRequest<BillResponse>(
                null,
                validation.Errors.First().ErrorMessage,
                validation.Errors.Select(e => e.ErrorMessage).ToList());
        }

        // Merge duplicate lines for the same product so the stock check and bill total are correct.
        var quantities = request.Items
            .GroupBy(i => i.ProductId)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));

        // Load every product and validate stock BEFORE writing anything — a bill either fully
        // succeeds or nothing changes (Rule 6 / Rule 7).
        var products = new Dictionary<int, Product>();
        foreach (var productId in quantities.Keys)
        {
            var product = await _productRepository.GetTrackedByIdAsync(organizationId, productId, cancellationToken);
            if (product is null)
                return _response.BadRequest<BillResponse>(null, "One of the products in this bill no longer exists.");

            products[productId] = product;
        }

        foreach (var (productId, quantity) in quantities)
        {
            var product = products[productId];
            if (product.CurrentStock < quantity)
                return _response.BadRequest<BillResponse>(null, $"Only {product.CurrentStock} {product.Unit} available for {product.Name}.");
        }

        var bill = new Bill
        {
            OrganizationId = organizationId,
            CustomerName = string.IsNullOrWhiteSpace(request.CustomerName) ? null : request.CustomerName.Trim(),
            CustomerPhone = string.IsNullOrWhiteSpace(request.CustomerPhone) ? null : request.CustomerPhone.Trim()
        };

        decimal total = 0;
        foreach (var (productId, quantity) in quantities)
        {
            var product = products[productId];
            var lineTotal = product.SellingPrice * quantity;
            total += lineTotal;

            bill.Items.Add(new BillItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.SellingPrice,
                Quantity = quantity,
                LineTotal = lineTotal
            });

            var previousStock = product.CurrentStock;
            var newStock = previousStock - quantity;
            product.CurrentStock = newStock;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.AddStockMovementAsync(new StockMovement
            {
                OrganizationId = organizationId,
                ProductId = product.Id,
                Bill = bill, // Bill.Id doesn't exist yet — nav property lets EF resolve BillId once it's generated in this same save.
                Reason = StockMovementReason.Sale,
                QuantityDelta = -quantity,
                PreviousStock = previousStock,
                NewStock = newStock
            }, cancellationToken);
        }

        bill.Total = total;

        await _billRepository.AddAsync(bill, cancellationToken);
        await _billRepository.SaveChangesAsync(cancellationToken);

        return _response.Created(ToResponse(bill), "Bill created");
    }

    public async Task<ApiResponse<BillResponse>> CreateReturnAsync(int organizationId, int billId, CreateBillReturnRequest request, CancellationToken cancellationToken)
    {
        var validation = await _returnValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return _response.BadRequest<BillResponse>(
                null,
                validation.Errors.First().ErrorMessage,
                validation.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var bill = await _billRepository.GetTrackedByIdAsync(organizationId, billId, cancellationToken);
        if (bill is null)
            return _response.NotFound<BillResponse>("Bill not found.");

        var billItem = bill.Items.FirstOrDefault(i => i.Id == request.BillItemId);
        if (billItem is null)
            return _response.BadRequest<BillResponse>(null, "That item isn't on this bill.");

        var alreadyReturned = bill.Returns.Where(r => r.BillItemId == billItem.Id).Sum(r => r.ReturnedQuantity);
        var remaining = billItem.Quantity - alreadyReturned;
        if (request.ReturnedQuantity > remaining)
            return _response.BadRequest<BillResponse>(null, $"Only {remaining} of {billItem.ProductName} can still be returned on this bill.");

        var originalProduct = await _productRepository.GetTrackedByIdAsync(organizationId, billItem.ProductId, cancellationToken);
        if (originalProduct is null)
            return _response.BadRequest<BillResponse>(null, "The original product on this bill no longer exists.");

        // Returned units go back into stock regardless of whether this is a plain return or an exchange.
        var originalPreviousStock = originalProduct.CurrentStock;
        var originalNewStock = originalPreviousStock + request.ReturnedQuantity;
        originalProduct.CurrentStock = originalNewStock;
        originalProduct.UpdatedAt = DateTime.UtcNow;

        await _productRepository.AddStockMovementAsync(new StockMovement
        {
            OrganizationId = organizationId,
            ProductId = originalProduct.Id,
            BillId = bill.Id,
            Reason = StockMovementReason.Return,
            QuantityDelta = request.ReturnedQuantity,
            PreviousStock = originalPreviousStock,
            NewStock = originalNewStock
        }, cancellationToken);

        Product? replacementProduct = null;
        if (request.ReplacementProductId.HasValue && request.ReplacementQuantity.HasValue)
        {
            replacementProduct = await _productRepository.GetTrackedByIdAsync(organizationId, request.ReplacementProductId.Value, cancellationToken);
            if (replacementProduct is null)
                return _response.BadRequest<BillResponse>(null, "The replacement product no longer exists.");

            if (replacementProduct.CurrentStock < request.ReplacementQuantity.Value)
                return _response.BadRequest<BillResponse>(null, $"Only {replacementProduct.CurrentStock} {replacementProduct.Unit} available for {replacementProduct.Name}.");

            var replacementPreviousStock = replacementProduct.CurrentStock;
            var replacementNewStock = replacementPreviousStock - request.ReplacementQuantity.Value;
            replacementProduct.CurrentStock = replacementNewStock;
            replacementProduct.UpdatedAt = DateTime.UtcNow;

            await _productRepository.AddStockMovementAsync(new StockMovement
            {
                OrganizationId = organizationId,
                ProductId = replacementProduct.Id,
                BillId = bill.Id,
                Reason = StockMovementReason.Exchange,
                QuantityDelta = -request.ReplacementQuantity.Value,
                PreviousStock = replacementPreviousStock,
                NewStock = replacementNewStock
            }, cancellationToken);
        }

        await _billRepository.AddReturnAsync(new BillReturn
        {
            OrganizationId = organizationId,
            BillId = bill.Id,
            BillItemId = billItem.Id,
            ReturnedQuantity = request.ReturnedQuantity,
            ReplacementProductId = replacementProduct?.Id,
            ReplacementQuantity = replacementProduct is null ? null : request.ReplacementQuantity,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
        }, cancellationToken);

        await _billRepository.SaveChangesAsync(cancellationToken);

        var refreshed = await _billRepository.GetByIdAsync(organizationId, billId, cancellationToken);
        return _response.Created(ToResponse(refreshed!), "Return recorded");
    }

    private static BillSummaryResponse ToSummary(Bill bill) => new()
    {
        Id = bill.Id,
        CustomerName = bill.CustomerName,
        ItemCount = bill.Items.Count,
        Total = bill.Total,
        CreatedAt = bill.CreatedAt
    };

    private static BillResponse ToResponse(Bill bill) => new()
    {
        Id = bill.Id,
        CustomerName = bill.CustomerName,
        CustomerPhone = bill.CustomerPhone,
        Total = bill.Total,
        CreatedAt = bill.CreatedAt,
        Items = bill.Items.Select(i => new BillItemResponse
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            UnitPrice = i.UnitPrice,
            Quantity = i.Quantity,
            LineTotal = i.LineTotal,
            ReturnedQuantity = bill.Returns.Where(r => r.BillItemId == i.Id).Sum(r => r.ReturnedQuantity)
        }).ToList(),
        Returns = bill.Returns.Select(r =>
        {
            // Looked up from bill.Items (already loaded in memory) rather than r.BillItem —
            // that navigation isn't part of the Include chain that loaded this bill.
            var sourceItem = bill.Items.First(i => i.Id == r.BillItemId);
            return new BillReturnResponse
            {
                Id = r.Id,
                BillItemId = r.BillItemId,
                ProductId = sourceItem.ProductId,
                ProductName = sourceItem.ProductName,
                ReturnedQuantity = r.ReturnedQuantity,
                ReplacementProductId = r.ReplacementProductId,
                ReplacementProductName = r.ReplacementProduct?.Name,
                ReplacementQuantity = r.ReplacementQuantity,
                Notes = r.Notes,
                CreatedAt = r.CreatedAt
            };
        }).OrderByDescending(r => r.CreatedAt).ToList()
    };
}
