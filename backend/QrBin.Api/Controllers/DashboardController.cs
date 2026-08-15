using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QrBin.Api.Managers;
using QrBin.Api.Services;

namespace QrBin.Api.Controllers;

[ApiController]
[Authorize(Roles = "Manager")]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardManager _dashboardManager;
    private readonly ICurrentManagerContext _currentManager;

    public DashboardController(IDashboardManager dashboardManager, ICurrentManagerContext currentManager)
    {
        _dashboardManager = dashboardManager;
        _currentManager = currentManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var response = await _dashboardManager.GetSummaryAsync(_currentManager.OrganizationId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
