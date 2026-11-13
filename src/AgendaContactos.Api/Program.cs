using AgendaContactos.Api;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddValidatorsFromAssemblyContaining<ContactRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateUserRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

var connectionString = builder.Configuration
    .GetConnectionString("ContactDatabase")
    ?? throw new InvalidOperationException(
        "Falta la variable ConnectionStrings__ContactDatabase.");

builder.Services.AddDbContext<ContactDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

if (allowedOrigins.Length > 0)
{
    builder.Services.AddCors(options => options.AddPolicy("Frontend", policy => policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (allowedOrigins.Length > 0)
{
    app.UseCors("Frontend");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }