using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaContactos.Api;

[ApiController]
[Route("api/[controller]")]
public sealed class UserController(IUserService userService, IValidator<RegisterUserRequest> validator, IValidator<UpdateUserRequest> updateValidator) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAllUsers(CancellationToken cancellationToken)
    {
        var user = await userService.GetAllUsers(cancellationToken);

        return Ok(user);
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetUserById(CancellationToken cancellationToken)
    {
        if (!AuthHelpers.TryGetCurrentUserId(User, out var userId))
            return Unauthorized();

        var user = await userService.GetUserById(userId, cancellationToken);

        return user is null ? NotFound() : Ok(user);
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

        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<UserResponse>> UpdateUser([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
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
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteUser(CancellationToken cancellationToken)
    {
        if (!AuthHelpers.TryGetCurrentUserId(User, out var userId))
            return Unauthorized();

        await userService.DeleteUser(userId, cancellationToken);

        return NoContent();
    }
}