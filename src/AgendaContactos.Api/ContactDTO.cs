namespace AgendaContactos.Api;

public sealed record ContactRequest(string Name, string Phone, string Email);

public sealed record ContactResponse(Guid Id, string Name, string Phone, string Email);