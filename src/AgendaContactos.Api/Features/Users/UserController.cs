using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaContactos.Api;

[ApiController]
[Route("api/[controller]")]
public sealed class UserController(IUserService userService, IValidator<RegisterUserRequest> validator, IValidator<UpdateUserRequest> updateValidator) : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetCurrentUser(
    CancellationToken cancellationToken)
    {
        if (!AuthHelpers.TryGetCurrentUserId(User, out var userId))
            return Unauthorized();

        var user = await userService.GetUserById(userId, cancellationToken);

        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var createdUser = await userService.CreateUser(request, cancellationToken);

        return CreatedAtAction(nameof(GetCurrentUser), new { id = createdUser.Id }, createdUser);
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<ActionResult<UserResponse>> UpdateCurrentUser([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        if (!AuthHelpers.TryGetCurrentUserId(User, out var userId))
            return Unauthorized();

        var validationResult = await updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var updatedUser = await userService.UpdateUser(userId, request, cancellationToken);

        return updatedUser is null ? NotFound() : Ok(updatedUser);
    }

    [Authorize]
    [HttpDelete("me")]
    public async Task<ActionResult> DeleteCurrentUser(CancellationToken cancellationToken)
    {
        if (!AuthHelpers.TryGetCurrentUserId(User, out var userId))
            return Unauthorized();

        await userService.DeleteUser(userId, cancellationToken);

        return NoContent();
    }
}