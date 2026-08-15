using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QrBin.Api.Managers;
using QrBin.Api.Services;

namespace QrBin.Api.Controllers;

[ApiController]
[Authorize(Roles = "Manager")]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryManager _inventoryManager;
    private readonly ICurrentManagerContext _currentManager;

    public InventoryController(IInventoryManager inventoryManager, ICurrentManagerContext currentManager)
    {
        _inventoryManager = inventoryManager;
        _currentManager = currentManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetParts(CancellationToken cancellationToken)
    {
        var response = await _inventoryManager.GetPartsAsync(_currentManager.OrganizationId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
