using Microsoft.AspNetCore.Mvc;
using QrBin.Api.Managers;
using QrBin.Api.Services;
using QrBin.ViewModels.Products;

namespace QrBin.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductManager _productManager;
    private readonly ICurrentOrganizationContext _currentOrg;

    public ProductsController(IProductManager productManager, ICurrentOrganizationContext currentOrg)
    {
        _productManager = productManager;
        _currentOrg = currentOrg;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var organizationId = await _currentOrg.GetOrganizationIdAsync(cancellationToken);
        var response = await _productManager.GetAllAsync(organizationId, search, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var organizationId = await _currentOrg.GetOrganizationIdAsync(cancellationToken);
        var response = await _productManager.GetByIdAsync(organizationId, id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var organizationId = await _currentOrg.GetOrganizationIdAsync(cancellationToken);
        var response = await _productManager.CreateAsync(organizationId, request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var organizationId = await _currentOrg.GetOrganizationIdAsync(cancellationToken);
        var response = await _productManager.UpdateAsync(organizationId, id, request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("{id:int}/stock")]
    public async Task<IActionResult> AddStock(int id, [FromBody] AddStockRequest request, CancellationToken cancellationToken)
    {
        var organizationId = await _currentOrg.GetOrganizationIdAsync(cancellationToken);
        var response = await _productManager.AddStockAsync(organizationId, id, request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id:int}/stock-history")]
    public async Task<IActionResult> GetStockHistory(int id, CancellationToken cancellationToken)
    {
        var organizationId = await _currentOrg.GetOrganizationIdAsync(cancellationToken);
        var response = await _productManager.GetStockHistoryAsync(organizationId, id, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
