namespace AgendaContactos.Api;

public sealed record LoginRequest(string Email, string Password);

public sealed record LoginResponse(string Token);