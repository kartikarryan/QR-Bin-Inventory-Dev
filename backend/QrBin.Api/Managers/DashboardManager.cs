using QrBin.Api.Common.Utility;
using QrBin.Data.Repositories;
using QrBin.ViewModels;
using QrBin.ViewModels.Dashboard;

namespace QrBin.Api.Managers;

public interface IDashboardManager
{
    Task<ApiResponse<DashboardResponse>> GetSummaryAsync(int organizationId, CancellationToken cancellationToken);
}

public class DashboardManager : IDashboardManager
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IApiResponseBuilder _response;

    public DashboardManager(IDashboardRepository dashboardRepository, IApiResponseBuilder response)
    {
        _dashboardRepository = dashboardRepository;
        _response = response;
    }

    public async Task<ApiResponse<DashboardResponse>> GetSummaryAsync(int organizationId, CancellationToken cancellationToken)
    {
        var summary = await _dashboardRepository.GetSummaryAsync(organizationId, cancellationToken);

        var result = new DashboardResponse
        {
            TotalParts = summary.TotalParts,
            LowStockCount = summary.LowStockCount,
            OutOfStockCount = summary.OutOfStockCount,
            LowStockParts = summary.LowStockParts.Select(p => new LowStockPartResponse
            {
                Id = p.Id,
                PartName = p.PartName,
                CurrentStock = p.CurrentStock,
                MinimumStock = p.MinimumStock,
                BinCode = p.BinCode
            }).ToList()
        };

        return _response.Ok(result);
    }
}
