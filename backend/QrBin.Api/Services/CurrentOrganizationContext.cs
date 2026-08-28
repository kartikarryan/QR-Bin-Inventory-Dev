using Microsoft.EntityFrameworkCore;
using QrBin.Data.Context;

namespace QrBin.Api.Services;

/// <summary>
/// Resolves the organization every request operates on. There's no login anymore, so this is
/// not "the authenticated manager's org" — it's whichever Organization exists in the database.
/// Only correct as long as this deployment serves a single business; revisit before onboarding
/// a second customer onto the same database.
/// </summary>
public interface ICurrentOrganizationContext
{
    Task<int> GetOrganizationIdAsync(CancellationToken cancellationToken = default);
}

public class CurrentOrganizationContext : ICurrentOrganizationContext
{
    private readonly QrBinDbContext _context;

    public CurrentOrganizationContext(QrBinDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetOrganizationIdAsync(CancellationToken cancellationToken = default)
    {
        var id = await _context.Organizations
            .OrderBy(o => o.Id)
            .Select(o => o.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (id == 0)
            throw new InvalidOperationException("No organization exists yet — seed one before using the API.");

        return id;
    }
}
