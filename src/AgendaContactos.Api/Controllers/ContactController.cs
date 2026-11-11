using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace AgendaContactos.Api;

[ApiController]
[Route("api/[controller]")]
public sealed class ContactController(IContactService contactService, IValidator<ContactRequest> validator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ContactResponse>>> GetAllContacts(CancellationToken cancellationToken)
    {
        var contacts = await contactService.GetAllContacts(cancellationToken);

        return Ok(contacts);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ContactResponse>> GetContactById(Guid id, CancellationToken cancellationToken)
    {
        var contact = await contactService.GetContactById(id, cancellationToken);

        return contact is null ? NotFound() : Ok(contact);
    }

    [HttpPost]
    public async Task<ActionResult<ContactResponse>> CreateContact([FromBody] ContactRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var createdContact = await contactService.CreateContact(request, cancellationToken);

        return CreatedAtAction(nameof(GetContactById), new { id = createdContact.Id }, createdContact);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ContactResponse>> UpdateContact(Guid id, [FromBody] ContactRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var updatedContact = await contactService.UpdateContact(id, request, cancellationToken);

        return updatedContact is null ? NotFound() : Ok(updatedContact);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteContact(Guid id, CancellationToken cancellationToken)
    {
        await contactService.DeleteContact(id, cancellationToken);

        return NoContent();
    }
}