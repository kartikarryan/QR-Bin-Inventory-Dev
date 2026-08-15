using Microsoft.AspNetCore.Mvc;
using QrBin.Api.Managers;
using QrBin.ViewModels.Auth;

namespace QrBin.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthManager _authManager;

    public AuthController(IAuthManager authManager)
    {
        _authManager = authManager;
    }

    [HttpPost("manager/login")]
    public async Task<IActionResult> ManagerLogin([FromBody] ManagerLoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authManager.ManagerLoginAsync(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
