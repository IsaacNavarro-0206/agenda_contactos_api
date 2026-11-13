using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AgendaContactos.Api;

public interface IUserService
{
    Task<IReadOnlyList<UserResponse>> GetAllUsers(CancellationToken cancellationToken);

    Task<UserResponse> GetUserById(Guid id, CancellationToken cancellationToken);

    Task<UserResponse> CreateUser(RegisterUserRequest userRequest, CancellationToken cancellationToken);

    Task<UserResponse> UpdateUser(Guid id, UpdateUserRequest userRequest, CancellationToken cancellationToken);

    Task<bool> DeleteUser(Guid id, CancellationToken cancellationToken);
}

public sealed class UserService(ContactDbContext dbContext, IPasswordHasher<User> passwordHasher) : IUserService
{
    public async Task<IReadOnlyList<UserResponse>> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await dbContext.Users.AsNoTracking().ToListAsync(cancellationToken);

        return users.Select(MapToResponse).ToList();
    }

    public async Task<UserResponse> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException($"No se encontró un usuario con el ID {id}.");
        }

        return MapToResponse(user);
    }

    public async Task<UserResponse> CreateUser(RegisterUserRequest userRequest, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = userRequest.Name.Trim(),
            Email = userRequest.Email.Trim()
        };

        user.Password = passwordHasher.HashPassword(user, userRequest.Password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(user);
    }

    public async Task<UserResponse> UpdateUser(Guid id, UpdateUserRequest userRequest, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException($"No se encontró un usuario con el ID {id}.");
        }

        user.Name = userRequest.Name.Trim();
        user.Email = userRequest.Email.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(user);
    }

    public async Task<bool> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            return false;
        }

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static UserResponse MapToResponse(User user) =>
        new UserResponse(user.Id, user.Name, user.Email);

}