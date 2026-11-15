using Microsoft.EntityFrameworkCore;

namespace AgendaContactos.Api;

public interface IContactService
{
    Task<IReadOnlyList<ContactResponse>> GetAllContacts(Guid userId, CancellationToken cancellationToken);

    Task<ContactResponse?> GetContactById(Guid userId, Guid contactId, CancellationToken cancellationToken);

    Task<ContactResponse> CreateContact(Guid userId, ContactRequest request, CancellationToken cancellationToken);

    Task<ContactResponse?> UpdateContact(Guid userId, Guid contactId, ContactRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteContact(Guid userId, Guid contactId, CancellationToken cancellationToken);
}

public sealed class ContactService(ContactDbContext dbContext) : IContactService
{
    public async Task<IReadOnlyList<ContactResponse>> GetAllContacts(Guid userId, CancellationToken cancellationToken)
    {
        var contacts = await dbContext.Contacts
            .AsNoTracking()
            .Where(contact => contact.UserId == userId)
            .ToListAsync(cancellationToken);

        return contacts.Select(MapToResponse).ToList();
    }

    public async Task<ContactResponse?> GetContactById(
        Guid userId,
        Guid contactId,
        CancellationToken cancellationToken)
    {
        var contact = await dbContext.Contacts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                contact => contact.Id == contactId && contact.UserId == userId,
                cancellationToken);

        return contact is null ? null : MapToResponse(contact);
    }

    public async Task<ContactResponse> CreateContact(
        Guid userId,
        ContactRequest request,
        CancellationToken cancellationToken)
    {
        var contact = new Contact
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Phone = request.Phone.Trim(),
            Email = request.Email.Trim(),
            UserId = userId
        };

        dbContext.Contacts.Add(contact);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(contact);
    }

    public async Task<ContactResponse?> UpdateContact(
        Guid userId,
        Guid contactId,
        ContactRequest request,
        CancellationToken cancellationToken)
    {
        var contact = await dbContext.Contacts.FirstOrDefaultAsync(
            contact => contact.Id == contactId && contact.UserId == userId,
            cancellationToken);

        if (contact is null)
        {
            return null;
        }

        contact.Name = request.Name.Trim();
        contact.Phone = request.Phone.Trim();
        contact.Email = request.Email.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(contact);
    }

    public async Task<bool> DeleteContact(
        Guid userId,
        Guid contactId,
        CancellationToken cancellationToken)
    {
        var contact = await dbContext.Contacts.FirstOrDefaultAsync(
            contact => contact.Id == contactId && contact.UserId == userId,
            cancellationToken);

        if (contact is null)
        {
            return false;
        }

        dbContext.Contacts.Remove(contact);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static ContactResponse MapToResponse(Contact contact) =>
        new(contact.Id, contact.Name, contact.Phone, contact.Email, contact.UserId);
}