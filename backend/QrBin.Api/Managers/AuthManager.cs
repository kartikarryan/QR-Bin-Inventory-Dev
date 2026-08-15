using FluentValidation;
using Microsoft.EntityFrameworkCore;
using QrBin.Api.Common.Utility;
using QrBin.Api.Services;
using QrBin.Data.Context;
using QrBin.ViewModels;
using QrBin.ViewModels.Auth;

namespace QrBin.Api.Managers;

public interface IAuthManager
{
    Task<ApiResponse<ManagerLoginResponse>> ManagerLoginAsync(ManagerLoginRequest request, CancellationToken cancellationToken);
}

public class AuthManager : IAuthManager
{
    private readonly QrBinDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IValidator<ManagerLoginRequest> _validator;
    private readonly IApiResponseBuilder _response;

    public AuthManager(
        QrBinDbContext context,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IValidator<ManagerLoginRequest> validator,
        IApiResponseBuilder response)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _validator = validator;
        _response = response;
    }

    public async Task<ApiResponse<ManagerLoginResponse>> ManagerLoginAsync(ManagerLoginRequest request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return _response.BadRequest<ManagerLoginResponse>(
                null,
                validation.Errors.First().ErrorMessage,
                validation.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var manager = await _context.Managers
            .Include(m => m.Organization)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Email == request.Email, cancellationToken);

        if (manager is null || !_passwordHasher.VerifyPassword(manager.PasswordHash, request.Password))
            return _response.Unauthorized<ManagerLoginResponse>("Invalid email or password.");

        var (token, expiresAt) = _tokenService.CreateManagerToken(manager);

        var result = new ManagerLoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            Manager = new ManagerProfileResponse
            {
                Id = manager.Id,
                Email = manager.Email,
                FullName = manager.FullName,
                OrganizationId = manager.OrganizationId,
                OrganizationName = manager.Organization.Name
            }
        };

        return _response.Ok(result, "Login successful");
    }
}
