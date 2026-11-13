public sealed record UserResponse(Guid Id, string Name, string Email);

public sealed record RegisterUserRequest(
    string Name,
    string Email,
    string Password);

public sealed record UpdateUserRequest(string Name, string Email);