using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;

namespace AgendaContactos.Api;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class ContactController(IContactService contactService, IValidator<ContactRequest> validator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ContactResponse>>> GetAllContacts(CancellationToken cancellationToken)
    {
        if (!AuthHelpers.TryGetCurrentUserId(User, out var userId)) return Unauthorized();

        var contacts = await contactService.GetAllContacts(userId, cancellationToken);

        return Ok(contacts);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ContactResponse>> GetContactById(Guid id, CancellationToken cancellationToken)
    {
        if (!AuthHelpers.TryGetCurrentUserId(User, out var userId))
            return Unauthorized();

        var contact = await contactService.GetContactById(userId, id, cancellationToken);

        return contact is null ? NotFound() : Ok(contact);
    }

    [HttpPost]
    public async Task<ActionResult<ContactResponse>> CreateContact([FromBody] ContactRequest request, CancellationToken cancellationToken)
    {
        if (!AuthHelpers.TryGetCurrentUserId(User, out var userId))
            return Unauthorized();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var createdContact = await contactService.CreateContact(userId, request, cancellationToken);

        return CreatedAtAction(nameof(GetContactById), new { id = createdContact.Id }, createdContact);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ContactResponse>> UpdateContact(Guid id, [FromBody] ContactRequest request, CancellationToken cancellationToken)
    {
        if (!AuthHelpers.TryGetCurrentUserId(User, out var userId))
            return Unauthorized();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var updatedContact = await contactService.UpdateContact(userId, id, request, cancellationToken);

        return updatedContact is null ? NotFound() : Ok(updatedContact);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteContact(Guid id, CancellationToken cancellationToken)
    {
        if (!AuthHelpers.TryGetCurrentUserId(User, out var userId))
            return Unauthorized();

        var deleted = await contactService.DeleteContact(userId, id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}