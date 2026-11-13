using Microsoft.EntityFrameworkCore;

namespace AgendaContactos.Api;

public interface IContactService
{
    Task<IReadOnlyList<ContactResponse>> GetAllContacts(CancellationToken cancellationToken);

    Task<ContactResponse> GetContactById(Guid id, CancellationToken cancellationToken);

    Task<ContactResponse> CreateContact(ContactRequest request, CancellationToken cancellationToken);

    Task<ContactResponse> UpdateContact(Guid id, ContactRequest request, CancellationToken cancellationToken);

    Task DeleteContact(Guid id, CancellationToken cancellationToken);
}

public sealed class ContactService(ContactDbContext dbContext) : IContactService
{
    public async Task<IReadOnlyList<ContactResponse>> GetAllContacts(CancellationToken cancellationToken)
    {
        var contacts = await dbContext.Contacts.AsNoTracking().ToListAsync(cancellationToken);

        return contacts.Select(MapToResponse).ToList();
    }

    public async Task<ContactResponse> GetContactById(Guid id, CancellationToken cancellationToken)
    {
        var contact = await dbContext.Contacts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (contact is null)
        {
            throw new KeyNotFoundException($"No se encontró un contacto con el ID {id}.");
        }

        return MapToResponse(contact);
    }

    public async Task<ContactResponse> CreateContact(ContactRequest request, CancellationToken cancellationToken)
    {
        var contact = new Contact
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Phone = request.Phone.Trim(),
            Email = request.Email.Trim(),
            UserId = request.UserId
        };

        dbContext.Contacts.Add(contact);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(contact);
    }

    public async Task<ContactResponse> UpdateContact(Guid id, ContactRequest request, CancellationToken cancellationToken)
    {
        var contact = await dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (contact is null)
        {
            throw new KeyNotFoundException($"No se encontró un contacto con el ID {id}.");
        }

        contact.Name = request.Name.Trim();
        contact.Phone = request.Phone.Trim();
        contact.Email = request.Email.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(contact);
    }

    public async Task DeleteContact(Guid id, CancellationToken cancellationToken)
    {
        var contact = await dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (contact is null)
        {
            throw new KeyNotFoundException($"No se encontró un contacto con el ID {id}.");
        }

        dbContext.Contacts.Remove(contact);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static ContactResponse MapToResponse(Contact contact) =>
        new(contact.Id, contact.Name, contact.Phone, contact.Email, contact.UserId);
}