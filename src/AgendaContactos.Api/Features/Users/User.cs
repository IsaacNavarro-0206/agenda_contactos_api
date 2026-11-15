namespace AgendaContactos.Api;

public class User : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; } = string.Empty;


    // Permite acceder a los contactos del usuario.
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
