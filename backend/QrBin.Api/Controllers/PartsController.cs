using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QrBin.Api.Managers;
using QrBin.Api.Services;
using QrBin.ViewModels.Inventory;

namespace QrBin.Api.Controllers;

[ApiController]
[Authorize(Roles = "Manager")]
[Route("api/parts")]
public class PartsController : ControllerBase
{
    private readonly IPartManager _partManager;
    private readonly ICurrentManagerContext _currentManager;

    public PartsController(IPartManager partManager, ICurrentManagerContext currentManager)
    {
        _partManager = partManager;
        _currentManager = currentManager;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePartRequest request, CancellationToken cancellationToken)
    {
        var response = await _partManager.CreateAsync(_currentManager.OrganizationId, request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await _partManager.GetByIdAsync(_currentManager.OrganizationId, id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePartRequest request, CancellationToken cancellationToken)
    {
        var response = await _partManager.UpdateAsync(_currentManager.OrganizationId, id, request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("{id:int}/stock-movements")]
    public async Task<IActionResult> AdjustStock(int id, [FromBody] StockMovementRequest request, CancellationToken cancellationToken)
    {
        var response = await _partManager.AdjustStockAsync(_currentManager.OrganizationId, id, request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
