using QrBin.Api.Common.Utility;
using QrBin.Data.Repositories;
using QrBin.ViewModels;
using QrBin.ViewModels.Inventory;

namespace QrBin.Api.Managers;

public interface IInventoryManager
{
    Task<ApiResponse<List<InventoryPartResponse>>> GetPartsAsync(int organizationId, CancellationToken cancellationToken);
}

public class InventoryManager : IInventoryManager
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IApiResponseBuilder _response;

    public InventoryManager(IInventoryRepository inventoryRepository, IApiResponseBuilder response)
    {
        _inventoryRepository = inventoryRepository;
        _response = response;
    }

    public async Task<ApiResponse<List<InventoryPartResponse>>> GetPartsAsync(int organizationId, CancellationToken cancellationToken)
    {
        var parts = await _inventoryRepository.GetPartsAsync(organizationId, cancellationToken);

        var result = parts.Select(p => new InventoryPartResponse
        {
            Id = p.Id,
            PartName = p.PartName,
            PartNumber = p.PartNumber,
            BinCode = p.BinCode,
            QrToken = p.QrToken,
            CurrentStock = p.CurrentStock,
            MinimumStock = p.MinimumStock,
            Status = PartStatusCalculator.GetStatus(p.CurrentStock, p.MinimumStock)
        }).ToList();

        return _response.Ok(result);
    }
}
