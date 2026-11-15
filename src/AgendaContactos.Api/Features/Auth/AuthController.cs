using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AgendaContactos.Api;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IAuthService authService, IValidator<LoginRequest> validator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var login = await authService.Login(request, cancellationToken);

        return login is null ? NotFound() : Ok(login);
    }
}