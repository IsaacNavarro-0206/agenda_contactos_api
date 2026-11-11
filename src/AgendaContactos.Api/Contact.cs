using Microsoft.EntityFrameworkCore;

namespace AgendaContactos.Api;

public class Contact
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
}

public sealed class ContactDbContext(DbContextOptions<ContactDbContext> options) : DbContext(options)
{
    public DbSet<Contact> Contacts => Set<Contact>();
}