using Microsoft.AspNetCore.Mvc;
using QrBin.Api.Managers;
using QrBin.Api.Services;

namespace QrBin.Api.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryManager _inventoryManager;
    private readonly ICurrentOrganizationContext _currentOrg;

    public InventoryController(IInventoryManager inventoryManager, ICurrentOrganizationContext currentOrg)
    {
        _inventoryManager = inventoryManager;
        _currentOrg = currentOrg;
    }

    [HttpGet]
    public async Task<IActionResult> GetParts(CancellationToken cancellationToken)
    {
        var organizationId = await _currentOrg.GetOrganizationIdAsync(cancellationToken);
        var response = await _inventoryManager.GetPartsAsync(organizationId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
