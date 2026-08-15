using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QrBin.Api.Managers;
using QrBin.ViewModels.Inventory;

namespace QrBin.Api.Controllers;

/// <summary>Public, unauthenticated endpoints for the QR-scan technician flow — resolves everything from the bin's QR token, no manager login required.</summary>
[ApiController]
[AllowAnonymous]
[Route("api/scan")]
public class ScanController : ControllerBase
{
    private readonly IPartManager _partManager;

    public ScanController(IPartManager partManager)
    {
        _partManager = partManager;
    }

    [HttpGet("{qrToken}")]
    public async Task<IActionResult> GetByQrToken(string qrToken, CancellationToken cancellationToken)
    {
        var response = await _partManager.GetByQrTokenAsync(qrToken, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("{qrToken}/stock-movements")]
    public async Task<IActionResult> AdjustStock(string qrToken, [FromBody] StockMovementRequest request, CancellationToken cancellationToken)
    {
        var response = await _partManager.AdjustStockByQrTokenAsync(qrToken, request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
