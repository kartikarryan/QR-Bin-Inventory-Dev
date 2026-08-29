using Microsoft.AspNetCore.Mvc;
using QrBin.Api.Managers;
using QrBin.Api.Services;
using QrBin.ViewModels.Billing;

namespace QrBin.Api.Controllers;

[ApiController]
[Route("api/bills")]
public class BillsController : ControllerBase
{
    private readonly IBillManager _billManager;
    private readonly ICurrentOrganizationContext _currentOrg;

    public BillsController(IBillManager billManager, ICurrentOrganizationContext currentOrg)
    {
        _billManager = billManager;
        _currentOrg = currentOrg;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var organizationId = await _currentOrg.GetOrganizationIdAsync(cancellationToken);
        var response = await _billManager.GetAllAsync(organizationId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var organizationId = await _currentOrg.GetOrganizationIdAsync(cancellationToken);
        var response = await _billManager.GetByIdAsync(organizationId, id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBillRequest request, CancellationToken cancellationToken)
    {
        var organizationId = await _currentOrg.GetOrganizationIdAsync(cancellationToken);
        var response = await _billManager.CreateAsync(organizationId, request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("{id:int}/returns")]
    public async Task<IActionResult> CreateReturn(int id, [FromBody] CreateBillReturnRequest request, CancellationToken cancellationToken)
    {
        var organizationId = await _currentOrg.GetOrganizationIdAsync(cancellationToken);
        var response = await _billManager.CreateReturnAsync(organizationId, id, request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
