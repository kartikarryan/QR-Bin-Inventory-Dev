using Microsoft.AspNetCore.Mvc;
using QrBin.Api.Managers;
using QrBin.Api.Services;

namespace QrBin.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardManager _dashboardManager;
    private readonly ICurrentOrganizationContext _currentOrg;

    public DashboardController(IDashboardManager dashboardManager, ICurrentOrganizationContext currentOrg)
    {
        _dashboardManager = dashboardManager;
        _currentOrg = currentOrg;
    }

    [HttpGet]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var organizationId = await _currentOrg.GetOrganizationIdAsync(cancellationToken);
        var response = await _dashboardManager.GetSummaryAsync(organizationId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
