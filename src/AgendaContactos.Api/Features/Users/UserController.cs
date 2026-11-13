using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AgendaContactos.Api;

[ApiController]
[Route("api/[controller]")]
public sealed class UserController(IUserService userService, IValidator<RegisterUserRequest> validator, IValidator<UpdateUserRequest> updateValidator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAllUsers(CancellationToken cancellationToken)
    {
        var user = await userService.GetAllUsers(cancellationToken);

        return Ok(user);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var user = await userService.GetUserById(id, cancellationToken);

        return user is null ? NotFound() : Ok(user);
    }

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

    public async Task<ActionResult<UserResponse>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var updatedUser = await userService.UpdateUser(id, request, cancellationToken);

        return updatedUser is null ? NotFound() : Ok(updatedUser);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        await userService.DeleteUser(id, cancellationToken);

        return NoContent();
    }
}