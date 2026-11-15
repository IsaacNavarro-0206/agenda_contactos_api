namespace AgendaContactos.Api;

public class Contact : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
