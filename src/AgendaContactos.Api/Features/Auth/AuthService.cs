using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AgendaContactos.Api;

public interface IAuthService
{
    Task<LoginResponse> Login(LoginRequest request, CancellationToken cancellationToken);
}

public sealed class AuthService(ContactDbContext dbContext, IPasswordHasher<User> passwordHasher, IConfiguration configuration) : IAuthService
{
    private readonly string _jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Falta configurar Jwt:Key.");
    private readonly string _jwtIssuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Falta configurar Jwt:Issuer.");
    private readonly string _jwtAudience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Falta configurar Jwt:Audience.");

    public async Task<LoginResponse> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException($"No existe usuario con correo {request.Email}");
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var signingKey = new SymmetricSecurityKey(Convert.FromBase64String(_jwtKey));

        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new(tokenString);
    }
}