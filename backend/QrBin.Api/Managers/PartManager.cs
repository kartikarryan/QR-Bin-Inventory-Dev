using FluentValidation;
using QrBin.Api.Common.Utility;
using QrBin.Data.Entities;
using QrBin.Data.Repositories;
using QrBin.ViewModels;
using QrBin.ViewModels.Inventory;

namespace QrBin.Api.Managers;

public interface IPartManager
{
    Task<ApiResponse<InventoryPartResponse>> CreateAsync(int organizationId, CreatePartRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<InventoryPartResponse>> GetByIdAsync(int organizationId, int partId, CancellationToken cancellationToken);
    Task<ApiResponse<InventoryPartResponse>> UpdateAsync(int organizationId, int partId, UpdatePartRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<InventoryPartResponse>> AdjustStockAsync(int organizationId, int partId, StockMovementRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<InventoryPartResponse>> GetByQrTokenAsync(string qrToken, CancellationToken cancellationToken);
    Task<ApiResponse<InventoryPartResponse>> AdjustStockByQrTokenAsync(string qrToken, StockMovementRequest request, CancellationToken cancellationToken);
}

public class PartManager : IPartManager
{
    private readonly IPartRepository _partRepository;
    private readonly IValidator<CreatePartRequest> _createValidator;
    private readonly IValidator<UpdatePartRequest> _updateValidator;
    private readonly IValidator<StockMovementRequest> _stockMovementValidator;
    private readonly IApiResponseBuilder _response;

    public PartManager(
        IPartRepository partRepository,
        IValidator<CreatePartRequest> createValidator,
        IValidator<UpdatePartRequest> updateValidator,
        IValidator<StockMovementRequest> stockMovementValidator,
        IApiResponseBuilder response)
    {
        _partRepository = partRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _stockMovementValidator = stockMovementValidator;
        _response = response;
    }

    public async Task<ApiResponse<InventoryPartResponse>> CreateAsync(int organizationId, CreatePartRequest request, CancellationToken cancellationToken)
    {
        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return _response.BadRequest<InventoryPartResponse>(
                null,
                validation.Errors.First().ErrorMessage,
                validation.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var partNumber = string.IsNullOrWhiteSpace(request.PartNumber) ? null : request.PartNumber.Trim();
        if (partNumber is not null && await _partRepository.PartNumberExistsAsync(organizationId, partNumber, null, cancellationToken))
            return _response.Conflict<InventoryPartResponse>($"Part number '{partNumber}' is already in use.");

        var binCode = request.BinCode.Trim();
        var bin = await _partRepository.FindBinByCodeAsync(organizationId, binCode, cancellationToken);

        if (bin is not null)
        {
            var assignedPartId = await _partRepository.GetAssignedPartIdAsync(bin.Id, null, cancellationToken);
            if (assignedPartId is not null)
                return _response.Conflict<InventoryPartResponse>($"Bin '{binCode}' is already assigned to another part.");
        }
        else
        {
            bin = _partRepository.PrepareNewBin(organizationId, binCode);
        }

        var part = new Part
        {
            OrganizationId = organizationId,
            PartName = request.PartName.Trim(),
            PartNumber = partNumber,
            CurrentStock = request.CurrentStock,
            MinimumStock = request.MinimumStock,
            Bin = bin
        };

        await _partRepository.AddAsync(part, cancellationToken);
        await _partRepository.SaveChangesAsync(cancellationToken);

        return _response.Created(ToResponse(part, binCode), "Part created");
    }

    public async Task<ApiResponse<InventoryPartResponse>> GetByIdAsync(int organizationId, int partId, CancellationToken cancellationToken)
    {
        var part = await _partRepository.GetByIdAsync(organizationId, partId, cancellationToken);
        if (part is null)
            return _response.NotFound<InventoryPartResponse>("Part not found.");

        return _response.Ok(ToResponse(part, part.Bin?.Code));
    }

    public async Task<ApiResponse<InventoryPartResponse>> UpdateAsync(int organizationId, int partId, UpdatePartRequest request, CancellationToken cancellationToken)
    {
        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return _response.BadRequest<InventoryPartResponse>(
                null,
                validation.Errors.First().ErrorMessage,
                validation.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var part = await _partRepository.GetTrackedByIdAsync(organizationId, partId, cancellationToken);
        if (part is null)
            return _response.NotFound<InventoryPartResponse>("Part not found.");

        var partNumber = string.IsNullOrWhiteSpace(request.PartNumber) ? null : request.PartNumber.Trim();
        if (partNumber is not null && await _partRepository.PartNumberExistsAsync(organizationId, partNumber, partId, cancellationToken))
            return _response.Conflict<InventoryPartResponse>($"Part number '{partNumber}' is already in use.");

        var binCode = request.BinCode.Trim();
        if (part.Bin is null || part.Bin.Code != binCode)
        {
            var bin = await _partRepository.FindBinByCodeAsync(organizationId, binCode, cancellationToken);
            if (bin is not null)
            {
                var assignedPartId = await _partRepository.GetAssignedPartIdAsync(bin.Id, partId, cancellationToken);
                if (assignedPartId is not null)
                    return _response.Conflict<InventoryPartResponse>($"Bin '{binCode}' is already assigned to another part.");
            }
            else
            {
                bin = _partRepository.PrepareNewBin(organizationId, binCode);
            }

            part.Bin = bin;
        }

        part.PartName = request.PartName.Trim();
        part.PartNumber = partNumber;
        part.CurrentStock = request.CurrentStock;
        part.MinimumStock = request.MinimumStock;
        part.UpdatedAt = DateTime.UtcNow;

        await _partRepository.SaveChangesAsync(cancellationToken);

        return _response.Ok(ToResponse(part, binCode), "Part updated");
    }

    public async Task<ApiResponse<InventoryPartResponse>> AdjustStockAsync(int organizationId, int partId, StockMovementRequest request, CancellationToken cancellationToken)
    {
        var validation = await _stockMovementValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return _response.BadRequest<InventoryPartResponse>(
                null,
                validation.Errors.First().ErrorMessage,
                validation.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var part = await _partRepository.GetTrackedByIdAsync(organizationId, partId, cancellationToken);
        if (part is null)
            return _response.NotFound<InventoryPartResponse>("Part not found.");

        if (part.BinId is null)
            return _response.BadRequest<InventoryPartResponse>(null, "Assign this part to a bin before recording stock movements.");

        var previousStock = part.CurrentStock;
        var newStock = previousStock + request.QuantityDelta;
        if (newStock < 0)
            return _response.BadRequest<InventoryPartResponse>(null, $"Stock can't go below zero (currently {previousStock}).");

        part.CurrentStock = newStock;
        part.UpdatedAt = DateTime.UtcNow;

        await _partRepository.AddStockTransactionAsync(new StockTransaction
        {
            OrganizationId = organizationId,
            PartId = part.Id,
            BinId = part.BinId.Value,
            QuantityDelta = request.QuantityDelta,
            PreviousStock = previousStock,
            NewStock = newStock
        }, cancellationToken);

        await _partRepository.SaveChangesAsync(cancellationToken);

        return _response.Ok(ToResponse(part, part.Bin?.Code), "Stock updated");
    }

    public async Task<ApiResponse<InventoryPartResponse>> GetByQrTokenAsync(string qrToken, CancellationToken cancellationToken)
    {
        var bin = await _partRepository.GetBinByQrTokenAsync(qrToken, cancellationToken);
        if (bin is null)
            return _response.NotFound<InventoryPartResponse>("QR code not recognized.");

        var part = bin.Part;
        if (part is null)
            return _response.NotFound<InventoryPartResponse>("No part is assigned to this bin yet.");

        part.Bin = bin;
        return _response.Ok(ToResponse(part, bin.Code));
    }

    public async Task<ApiResponse<InventoryPartResponse>> AdjustStockByQrTokenAsync(string qrToken, StockMovementRequest request, CancellationToken cancellationToken)
    {
        var validation = await _stockMovementValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return _response.BadRequest<InventoryPartResponse>(
                null,
                validation.Errors.First().ErrorMessage,
                validation.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var bin = await _partRepository.GetTrackedBinByQrTokenAsync(qrToken, cancellationToken);
        if (bin is null)
            return _response.NotFound<InventoryPartResponse>("QR code not recognized.");

        var part = bin.Part;
        if (part is null)
            return _response.NotFound<InventoryPartResponse>("No part is assigned to this bin yet.");

        part.Bin = bin;

        var previousStock = part.CurrentStock;
        var newStock = previousStock + request.QuantityDelta;
        if (newStock < 0)
            return _response.BadRequest<InventoryPartResponse>(null, $"Stock can't go below zero (currently {previousStock}).");

        part.CurrentStock = newStock;
        part.UpdatedAt = DateTime.UtcNow;

        await _partRepository.AddStockTransactionAsync(new StockTransaction
        {
            OrganizationId = bin.OrganizationId,
            PartId = part.Id,
            BinId = bin.Id,
            QuantityDelta = request.QuantityDelta,
            PreviousStock = previousStock,
            NewStock = newStock
        }, cancellationToken);

        await _partRepository.SaveChangesAsync(cancellationToken);

        return _response.Ok(ToResponse(part, bin.Code), "Stock updated");
    }

    private static InventoryPartResponse ToResponse(Part part, string? binCode) => new()
    {
        Id = part.Id,
        PartName = part.PartName,
        PartNumber = part.PartNumber,
        BinCode = binCode,
        QrToken = part.Bin?.QrToken,
        CurrentStock = part.CurrentStock,
        MinimumStock = part.MinimumStock,
        Status = PartStatusCalculator.GetStatus(part.CurrentStock, part.MinimumStock)
    };
}
