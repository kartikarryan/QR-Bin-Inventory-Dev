using Microsoft.AspNetCore.Mvc;
using QrBin.Api.Managers;
using QrBin.Api.Services;
using QrBin.ViewModels.Inventory;

namespace QrBin.Api.Controllers;

[ApiController]
[Route("api/parts")]
public class PartsController : ControllerBase
{
    private readonly IPartManager _partManager;
    private readonly ICurrentOrganizationContext _currentOrg;

    public PartsController(IPartManager partManager, ICurrentOrganizationContext currentOrg)
    {
        _partManager = partManager;
        _currentOrg = currentOrg;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePartRequest request, CancellationToken cancellationToken)
    {
        var organizationId = await _currentOrg.GetOrganizationIdAsync(cancellationToken);
        var response = await _partManager.CreateAsync(organizationId, request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var organizationId = await _currentOrg.GetOrganizationIdAsync(cancellationToken);
        var response = await _partManager.GetByIdAsync(organizationId, id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePartRequest request, CancellationToken cancellationToken)
    {
        var organizationId = await _currentOrg.GetOrganizationIdAsync(cancellationToken);
        var response = await _partManager.UpdateAsync(organizationId, id, request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("{id:int}/stock-movements")]
    public async Task<IActionResult> AdjustStock(int id, [FromBody] StockMovementRequest request, CancellationToken cancellationToken)
    {
        var organizationId = await _currentOrg.GetOrganizationIdAsync(cancellationToken);
        var response = await _partManager.AdjustStockAsync(organizationId, id, request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
